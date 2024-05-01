using Merlion.Base;
using Merlion.Component.Dialogs;
using Merlion.Component.Inners;
using Merlion.Database;
using Merlion.Pages.Materials.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Diagnostics.Contracts;

namespace Merlion.Pages.Materials.Pages
{
    public partial class MaterialCategoryList : ComponentBase
    {
        public List<VisualMaterialCategory> Items { get; set; }
        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            InitializeTables();
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            Items = context.Set<MaterialCategory>().Select(x => new VisualMaterialCategory()
            {
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive,
                Description = x.Description
            }).ToList();

        }
        private async Task SaveMaterialCategory(JointContext<VisualMaterialCategory> jtContext)
        {
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            switch (jtContext.OperationType)
            {
                case "Edit":
                    var item = context.Set<MaterialCategory>().FirstOrDefault(x => x.Code == jtContext.DataModel!.Code);
                    if (item != null)
                    {
                        item.Code = jtContext.DataModel!.Code;
                        item.Name = jtContext.DataModel!.Name;
                        item.IsActive = jtContext.DataModel!.IsActive;
                        item.Description = jtContext.DataModel!.Description;
                        await context.SaveChangesWithAuditAsync();
                        var record = Items.FirstOrDefault(x => x.Code == jtContext.DataModel.Code);
                        if (record is not null)
                        {
                            record.Name = item.Name;
                            record.IsActive = item.IsActive;
                            record.Description = item.Description;
                        }
                    }
                    break;
                case "New":
                    MaterialCategory newItem = new MaterialCategory();
                    newItem.Code = jtContext.DataModel!.Code;
                    newItem.Name = jtContext.DataModel!.Name;
                    newItem.IsActive = jtContext.DataModel!.IsActive;
                    newItem.Description = jtContext.DataModel!.Description;
                    context.Set<MaterialCategory>().Add(newItem);
                    await context.SaveChangesWithAuditAsync();
                    VisualMaterialCategory newRecord = new VisualMaterialCategory();
                    Utilities.CopyPropertiesFrom<VisualMaterialCategory, MaterialCategory>(newRecord, newItem);
                    //var newRecord = Utilities.ForceCopyToSubInstance<MaterialCategory, VisualMaterialCategory>(newItem);
                    Items.Add(newRecord);
                    break;
            }
            _snackbarService.Add(Loc["The operation is completed successfully!"], Severity.Success);

            jtContext.CloseDialog();
        }

        private async Task DeleteMaterialCategory(JointContext<VisualMaterialCategory> jtContext)
        {
            await _dialogService.ShowConfirmDialog(Loc["Delete"], Loc["Are you sure to delete"], null, jtContext.DataModel.Code,
                                   async (CommonDialogEventArgs e) =>
                                   {
                                       using var context = await _pdbFactory.CreateDbContextAsync(this);
                                       bool exist = context.Set<MaterialMaster>().Any(x => x.Code == jtContext.DataModel.Code);
                                       if (exist)
                                       {
                                           this._snackbarService.Add(Loc["Some material masters are still running under this customer, the operation is cancelled!"], Severity.Error);
                                       }
                                       else
                                       {
                                           var customer = context.Set<MaterialCategory>().FirstOrDefault(x => x.Code == jtContext.DataModel.Code);
                                           if (customer != null)
                                           {
                                               context.Remove<MaterialCategory>(customer);
                                               await context.SaveChangesWithAuditAsync();
                                               Items.RemoveAll(x=>x.Code == jtContext.DataModel.Code);
                                               this._snackbarService.Add(Loc["This data has been removed successfully!"], Severity.Success);
                                           }
                                           else
                                           {
                                               this._snackbarService.Add(Loc["This data cannot be found, the operation is cancelled!"], Severity.Error);
                                           }
                                       }
                                   });
        }
        private void InitializeTables()
        {
            var cc = new MaterialCategoryDBInit();
            cc.ConnectionString = "server=localhost;database=AOM2;uid=postgres;pwd=postgres";
            cc.AddEntity<MaterialCategory>();
            cc.AddEntity<MaterialMaster>();
            cc.EnsureTablesCreated();
        }
    }
    class MaterialCategoryDBInit : InitializationDbContext { }


}
