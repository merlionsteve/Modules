using Merlion.Base;
using Merlion.Component.Dialogs;
using Merlion.Component.Inners;
using Merlion.Pages.Materials.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Reflection;

namespace Merlion.Pages.Materials.Pages
{
    public partial class MaterialMasterList : ComponentBase
    {
        public List<MaterialCategory> Categories { get; set; } = new List<MaterialCategory>();
        public MaterialCategory SelectedCategory { get; set; }
        public List<VisualMaterialMaster> vMaterialMasters { get; set; } = new List<VisualMaterialMaster>();

        protected override async Task OnInitializedAsync()
        {
            Categories = await this.getAllActvieMaterialCategories();
            base.OnInitializedAsync();
        }

        private async Task<List<MaterialCategory>> getAllActvieMaterialCategories()
        {
            using (var context = await _pdbFactory.CreateDbContextAsync(this))
            {
                Categories = context.Set<MaterialCategory>().Where(x => x.IsActive).OrderBy(x=>x.Name).ToList();
            }
            return Categories;
        }
        private async Task SelectedCategoryChanged(object selectedObj)
        {
            SelectedCategory = selectedObj as MaterialCategory;
            string catCode = "";
            if (SelectedCategory is not null)
            {
                catCode = SelectedCategory.Code;
            }
            using (var context = await _pdbFactory.CreateDbContextAsync(this))
            {
                var materials = context.Set<MaterialMaster>().Where(x => x.CategoryCode == catCode).OrderByDescending(x => x.ValidFrom).ToList();
                vMaterialMasters.Clear();
                foreach (var material in materials)
                {
                    VisualMaterialMaster newRecord = new VisualMaterialMaster();
                    Utilities.CopyPropertiesFrom<VisualMaterialMaster, MaterialMaster>(newRecord, material);
                    var cat = Categories.FirstOrDefault(x => x.Code == catCode);
                    if (cat is not null)
                    {
                        newRecord.CategoryName = cat.Name;
                    }
                    vMaterialMasters.Add(newRecord);
                }
                //filter call
            }
        }

        private async Task SaveMaterialMaster(JointContext<VisualMaterialMaster> jtContext)
        {
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            switch (jtContext.OperationType)
            {
                case "Edit":
                    var item = context.Set<MaterialMaster>().FirstOrDefault(x => x.Code == jtContext.DataModel!.Code);
                    if (item != null)
                    {
                        Utilities.CopyPropertiesFrom<MaterialMaster, VisualMaterialMaster>(item, jtContext.DataModel!);
                        await context.SaveChangesWithAuditAsync();
                        var record = vMaterialMasters.FirstOrDefault(x => x.Code == jtContext.DataModel!.Code);
                        if (item.CategoryCode == SelectedCategory.Code)
                        {
                            if (record is not null)
                            {
                                Utilities.CopyPropertiesFrom<VisualMaterialMaster, VisualMaterialMaster>(record, jtContext.DataModel!);
                            }
                        }
                        else
                        {
                            vMaterialMasters.Remove(record);
                        }
                    }
                    break;
                case "New":
                    MaterialMaster newItem = new MaterialMaster();
                    Utilities.CopyPropertiesFrom<MaterialMaster, VisualMaterialMaster>(newItem, jtContext.DataModel!);
                    context.Set<MaterialMaster>().Add(newItem);
                    await context.SaveChangesWithAuditAsync();
                    if (newItem.CategoryCode == SelectedCategory.Code)
                    {
                        VisualMaterialMaster vNewRecord = Utilities.CopyNewInstance<VisualMaterialMaster>(jtContext.DataModel!);                      
                        if (SelectedCategory is not null)
                        {
                            vNewRecord.CategoryName = SelectedCategory.Name;
                        }
                        vMaterialMasters.Add(vNewRecord);
                    }
                    break;
            }
            _snackbarService.Add(Loc["The operation is completed successfully!"], Severity.Success);

            jtContext.CloseDialog();
        }

        private async Task DeleteMaterialMaster(JointContext<VisualMaterialMaster> jtContext)
        {
            await _dialogService.ShowConfirmDialog(Loc["Delete"], Loc["Are you sure to delete"], null, jtContext.DataModel!.Code,
                                   async (CommonDialogEventArgs e) =>
                                   {
                                       using var context = await _pdbFactory.CreateDbContextAsync(this);
                                       //change to Materials in stead of MaterialMaster
                                       bool exist = context.Set<MaterialMaster>().Any(x => x.Code == jtContext.DataModel.Code);
                                       if (exist)
                                       {
                                           this._snackbarService.Add(Loc["Some contracts are still running under this materialMaster, the operation is cancelled!"], Severity.Error);
                                       }
                                       else
                                       {
                                           var materialMaster = context.Set<MaterialMaster>().FirstOrDefault(x => x.Code == jtContext.DataModel.Code);
                                           if (materialMaster != null)
                                           {
                                               context.Remove<MaterialMaster>(materialMaster);
                                               await context.SaveChangesWithAuditAsync();
                                               vMaterialMasters.RemoveAll(x => x.Code == jtContext.DataModel.Code);
                                               this._snackbarService.Add(Loc["This data has been removed successfully!"], Severity.Success);
                                           }
                                           else
                                           {
                                               this._snackbarService.Add(Loc["This data cannot be found, the operation is cancelled!"], Severity.Error);
                                           }
                                       }
                                   });
        }

    }
}
