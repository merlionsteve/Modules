using Merlion.Base;
using Merlion.Component.Dialogs;
using Merlion.Component.Inners;
using Merlion.Database;
using Merlion.Pages.ProductionModels.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Connections;
using MudBlazor;
using MudBlazor.Charts;
using System.Linq;
using System.Reflection;

namespace Merlion.Pages.ProductionModels.Pages
{
    public partial class BOMList : ComponentBase
    {
        public List<VisualBOM> vBOMs { get; set; } = new List<VisualBOM>();
        public VisualBOM SelectedBOM { get; set; } = null;

        public List<VisualBOMComponent> vBOMComponents { get; set; } = new List<VisualBOMComponent>();
        public List<MaterialMaster> CacheOfMaterials { get; set; }
        public VisualBOMComponent SelectedBOMComponent { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            InitializeTables();
            await this.cacheMaterials();
            await this.getAllBOMs();
            //Arms = await this.getAllActiveArms();
            base.OnInitializedAsync();
        }
        private async Task cacheMaterials()
        {
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            this.CacheOfMaterials = context.Set<MaterialMaster>().OrderBy(x => x.ShortDescription).ToList();
        }

        private async Task getAllBOMs()
        {
            using (var context = await _pdbFactory.CreateDbContextAsync(this))
            {
                var boms = context.Set<BillOfMaterial>().ToList();
                boms.ForEach(x =>
                {
                    var newBOM = new VisualBOM()
                    {
                        Code = x.Code,
                        Alternative = x.Alternative,
                        BaseQuantity = x.BaseQuantity,
                        ProductCode = x.ProductCode,
                        IsActive = x.IsActive
                    };
                    var mat = CacheOfMaterials.FirstOrDefault(y => y.Code == x.ProductCode);
                    if (mat != null)
                    {
                        newBOM.Product = mat.ShortDescription;
                        newBOM.Unit = mat.BaseUnit;
                    }
                    vBOMs.Add(newBOM);
                });
            }
        }

        private async Task SaveBOM(JointContext<VisualBOM> jtContext)
        {
            bool result = true;
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            switch (jtContext.OperationType)
            {
                case "Edit":
                    var item = context.Set<BillOfMaterial>().FirstOrDefault(x => x.Code.ToLower() == jtContext.DataModel!.Code.ToLower());
                    if (item != null)
                    {
                        item.Alternative = jtContext.DataModel!.Alternative;
                        item.ProductCode = jtContext.DataModel!.ProductCode;
                        item.BaseQuantity = jtContext.DataModel!.BaseQuantity;
                        item.IsActive = jtContext.DataModel!.IsActive;
                        await context.SaveChangesWithAuditAsync();
                        var record = vBOMs.FirstOrDefault(x => x.Code == jtContext.DataModel.Code);
                        if (record is not null)
                        {
                            record.Alternative = item.Alternative;
                            record.ProductCode = item.ProductCode;
                            record.BaseQuantity = item.BaseQuantity;
                            record.IsActive = item.IsActive;
                            var mat = CacheOfMaterials.FirstOrDefault(y => y.Code == record.ProductCode);
                            if (mat != null)
                            {
                                record.Product = mat.ShortDescription;
                                record.Unit = mat.BaseUnit;
                            }
                        }
                    }
                    else
                    {
                        result = false;
                        _snackbarService.Add(Loc["The data is not found!"], Severity.Error);
                    }
                    break;
                case "New":
                    var existedItem = context.Set<BillOfMaterial>().FirstOrDefault(x => x.Code.ToLower() == jtContext.DataModel!.Code.ToLower());
                    if (existedItem is null)
                    {
                        existedItem = context.Set<BillOfMaterial>().FirstOrDefault(x => x.ProductCode == jtContext.DataModel!.ProductCode && x.Alternative == jtContext.DataModel!.Alternative);
                        if (existedItem is null)
                        {
                            BillOfMaterial newItem = new BillOfMaterial();
                            newItem.Code = jtContext.DataModel!.Code;
                            newItem.ProductCode = jtContext.DataModel!.ProductCode;
                            newItem.Alternative = jtContext.DataModel!.Alternative;
                            newItem.BaseQuantity = jtContext.DataModel!.BaseQuantity;
                            newItem.IsActive = jtContext.DataModel!.IsActive;
                            context.Set<BillOfMaterial>().Add(newItem);
                            await context.SaveChangesWithAuditAsync();
                            VisualBOM newRecord = new VisualBOM();
                            Utilities.CopyPropertiesFrom<VisualBOM, BillOfMaterial>(newRecord, newItem);
                            var mat = CacheOfMaterials.FirstOrDefault(y => y.Code == newItem.ProductCode);
                            if (mat != null)
                            {
                                newRecord.Product = mat.ShortDescription;
                                newRecord.Unit = mat.BaseUnit;
                            }
                            vBOMs.Add(newRecord);
                        }
                        else
                        {
                            result = false;
                            _snackbarService.Add(Loc["The BOM with the same alternative for this product exists already!"], Severity.Error);
                        }
                    }
                    else
                    {
                        result = false;
                        _snackbarService.Add(Loc["The data exists already!"], Severity.Error);
                    }
                    break;
            }
            if (result)
            {
                _snackbarService.Add(Loc["The operation is completed successfully!"], Severity.Success);
                jtContext.CloseDialog();
            }
        }

        private async Task SaveBOMComponent(JointContext<VisualBOMComponent> jtContext)
        {
            bool result = true;
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            switch (jtContext.OperationType)
            {
                case "Edit":
                    var item = context.Set<BOMComponent>().FirstOrDefault(x => x.BOMCode == jtContext.DataModel!.BOMCode && x.ComponentCode == jtContext.DataModel!.ComponentCode);
                    if (item != null)
                    {
                        item.Quantity = jtContext.DataModel!.Quantity;
                        await context.SaveChangesWithAuditAsync();
                        var record = vBOMComponents.FirstOrDefault(x => x.BOMCode == jtContext.DataModel!.BOMCode && x.ComponentCode == jtContext.DataModel!.ComponentCode);
                        if (record is not null)
                        {
                            record.Quantity = item.Quantity;
                        }
                    }
                    else
                    {
                        result = false;
                        _snackbarService.Add(Loc["The data is not found!"], Severity.Error);
                    }
                    break;
                case "New":
                    var existedItem = context.Set<BOMComponent>().FirstOrDefault(x => x.ComponentCode == jtContext.DataModel!.ComponentCode);
                    if (existedItem is null)
                    {
                        BOMComponent newItem = new BOMComponent();
                        newItem.BOMCode = jtContext.DataModel!.BOMCode;
                        newItem.ComponentCode = jtContext.DataModel!.ComponentCode;
                        newItem.Quantity = jtContext.DataModel!.Quantity;
                        context.Set<BOMComponent>().Add(newItem);
                        await context.SaveChangesWithAuditAsync();

                        VisualBOMComponent newRecord = new VisualBOMComponent();
                        Utilities.CopyPropertiesFrom<VisualBOMComponent, BOMComponent>(newRecord, newItem);
                        var mat = CacheOfMaterials.FirstOrDefault(x => x.Code == newRecord.ComponentCode);
                        newRecord.Unit = mat?.BaseUnit;
                        newRecord.Component = mat?.ShortDescription;
                        vBOMComponents.Add(newRecord);
                    }
                    else
                    {
                        result = false;
                        _snackbarService.Add(Loc["The data exists already!"], Severity.Error);
                    }
                    //jtContext.DataModel.ComponentCode = null;
                    break;
            }
            if (result)
            {
                _snackbarService.Add(Loc["The operation is completed successfully!"], Severity.Success);
                jtContext.CloseDialog();
            }
        }

        private async Task DeleteBOM(JointContext<VisualBOM> jtContext)
        {
            await _dialogService.ShowConfirmDialog(Loc["Delete"], Loc["Are you sure to delete"], null, jtContext.DataModel.Code,
                                   async (CommonDialogEventArgs e) =>
                                   {
                                       using var context = await _pdbFactory.CreateDbContextAsync(this);
                                       bool exist = context.Set<BOMComponent>().Any(x => x.BOMCode == jtContext.DataModel.Code);
                                       if (exist)
                                       {
                                           this._snackbarService.Add(Loc["Some components are under this BOM, the operation is cancelled!"], Severity.Error);
                                       }
                                       else
                                       {
                                           var bom = context.Set<BillOfMaterial>().FirstOrDefault(x => x.Code == jtContext.DataModel.Code);
                                           if (bom != null)
                                           {
                                               context.Remove<BillOfMaterial>(bom);
                                               await context.SaveChangesWithAuditAsync();
                                               vBOMs.RemoveAll(x => x.Code == jtContext.DataModel.Code);
                                               this._snackbarService.Add(Loc["This data has been removed successfully!"], Severity.Success);
                                           }
                                           else
                                           {
                                               this._snackbarService.Add(Loc["This data is not found, the operation is cancelled!"], Severity.Error);
                                           }
                                       }
                                   });

        }

        private async Task DeleteBOMComponent(JointContext<VisualBOMComponent> jtContext)
        {
            await _dialogService.ShowConfirmDialog(Loc["Delete"], Loc["Are you sure to delete"], null, $"{jtContext.DataModel.BOMCode} : {jtContext.DataModel.ComponentCode}",
                                   async (CommonDialogEventArgs e) =>
                                   {
                                       using var context = await _pdbFactory.CreateDbContextAsync(this);
                                       var bomComponent = context.Set<BOMComponent>().FirstOrDefault(x => x.BOMCode == jtContext.DataModel.BOMCode && x.ComponentCode == jtContext.DataModel.ComponentCode);
                                       if (bomComponent != null)
                                       {
                                           context.Remove<BOMComponent>(bomComponent);
                                           await context.SaveChangesWithAuditAsync();
                                           vBOMComponents.RemoveAll(x => x.BOMCode == jtContext.DataModel.BOMCode && x.ComponentCode == jtContext.DataModel.ComponentCode);
                                           this._snackbarService.Add(Loc["This data has been removed successfully!"], Severity.Success);
                                       }
                                       else
                                       {
                                           this._snackbarService.Add(Loc["This data is not found, the operation is cancelled!"], Severity.Error);
                                       }
                                   });

        }

        private async Task SelectedBOMChanged(object obj)
        {
            if (obj is VisualBOM bom)
            {
                //SelectedBay = bomComponent;
                SelectedBOM = bom;
                if (bom is not null)
                {
                    using var context = await _pdbFactory.CreateDbContextAsync(this);
                    this.vBOMComponents = context.Set<BOMComponent>().Where(x => x.BOMCode == bom.Code).Select(x =>
                        new VisualBOMComponent()
                        {
                            BOMCode = x.BOMCode,
                            ComponentCode = x.ComponentCode,
                            Quantity = x.Quantity
                        }
                    ).ToList();
                    foreach (var component in this.vBOMComponents)
                    {
                        var com = CacheOfMaterials.FirstOrDefault(y => y.Code == component.ComponentCode);
                        if (com is not null)
                        {
                            component.Component = com.ShortDescription;
                            component.Unit = com.BaseUnit;
                        }
                    }
                }
            }
            else
            {
                SelectedBOM = null;
            }
            SelectedBOMComponent = null;
        }

        private async Task SelectedBOMComponentChanged(object obj)
        {
            //SelectedBay = bomComponent;
            if (obj is VisualBOMComponent bomComponent)
            {
                SelectedBOMComponent = bomComponent;
            }
            else
            {
                SelectedBOMComponent = null;
            }
        }

        private void InitializeTables()
        {
            var cc = new BOMDBInit();
            cc.ConnectionString = "server=localhost;database=AOM2;uid=postgres;pwd=postgres";
            cc.AddEntity<BillOfMaterial>();
            cc.AddEntity<BOMComponent>();
            cc.EnsureTablesCreated();
        }
    }

    class BOMDBInit : InitializationDbContext { }
}
