using Merlion.Database;
using Microsoft.AspNetCore.Components;
using Merlion.Pages.Customers.Entities;
using Merlion.Database.Entities;
using Merlion.Component.Inners;
using Merlion.Base;
using MudBlazor;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.Xml;
using Merlion.Component.Dialogs;
namespace Merlion.Pages.Customers.Pages
{
    public partial class CustomerList : ComponentBase
    {
        public List<VisualCustomer> Items { get; set; }
        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();           
            //InitializeTables();
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            Items = context.Set<Customer>().Select(x => new VisualCustomer()
            {
                Code = x.Code,
                Address = x.Address,
                Contact = x.Contact,
                Contacter = x.Contacter,
                Name = x.Name,
                Remark = x.Remark
            }).ToList();

        }
        private async Task SaveCustomer(JointContext<VisualCustomer> jtContext)
        {
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            switch (jtContext.OperationType)
            {
                case "Edit":
                    var item = context.Set<Customer>().FirstOrDefault(x => x.Code == jtContext.DataModel!.Code);
                    if (item != null)
                    {
                        item.Name = jtContext.DataModel!.Name;
                        item.Code = jtContext.DataModel!.Code;
                        item.Address = jtContext.DataModel!.Address;
                        item.Contacter = jtContext.DataModel!.Contacter;
                        item.Contact =  jtContext.DataModel!.Contact;
                        item.Remark = jtContext.DataModel!.Remark;
                        await context.SaveChangesWithAuditAsync();
                        var record = Items.FirstOrDefault(x => x.Code == jtContext.DataModel.Code);
                        if (record is not null)
                        {
                            //failed, it's a new obj
                            //record = Utilities.ForceCopyToSubInstance<Customer, VisualCustomer>(item);
                            record.Address = item.Address;
                            record.Contacter = item.Contacter;
                            record.Contact = item.Contact;
                            record.Name = item.Name;
                            record.Remark = item.Remark;

                        }
                    }
                    break;
                case "New":
                    Customer newItem = new Customer();
                    newItem.Name = jtContext.DataModel!.Name;
                    newItem.Code = jtContext.DataModel!.Code;
                    newItem.Address = jtContext.DataModel!.Address;
                    newItem.Contacter = jtContext.DataModel!.Contacter;
                    newItem.Contact = jtContext.DataModel!.Contact;
                    newItem.Remark = jtContext.DataModel!.Remark;
                    context.Set<Customer>().Add(newItem);
                    await context.SaveChangesWithAuditAsync();
                    VisualCustomer newRecord = new VisualCustomer();
                    Utilities.CopyPropertiesFrom<VisualCustomer, Customer>(newRecord, newItem);
                    //var newRecord = Utilities.ForceCopyToSubInstance<Customer, VisualCustomer>(newItem);
                    Items.Add(newRecord);
                    break;
            }
            _snackbarService.Add(Loc["The operation is completed successfully!"], Severity.Success);

            jtContext.CloseDialog();
        }

        private async Task DeleteCustomer(JointContext<VisualCustomer> jtContext)
        {
            await _dialogService.ShowConfirmDialog(Loc["Delete"], Loc["Are you sure to delete"], null, jtContext.DataModel.Code,
                                   async (CommonDialogEventArgs e) =>
                                   {
                                       using var context = await _pdbFactory.CreateDbContextAsync(this);
                                       bool exist = context.Set<Contract>().Any(x => x.CustomerCode == jtContext.DataModel.Code);
                                       if (exist)
                                       {
                                           this._snackbarService.Add(Loc["Some contracts are still running under this customer, the operation is cancelled!"], Severity.Error);
                                       }
                                       else
                                       {
                                           var customer = context.Set<Customer>().FirstOrDefault(x => x.Code == jtContext.DataModel.Code);
                                           if (customer != null)
                                           {
                                               context.Remove<Customer>(customer);
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
            var cc = new CustomerDBInit();
            cc.ConnectionString = "server=localhost;database=AOM2;uid=postgres;pwd=postgres";
            cc.AddEntity<Customer>();
            cc.AddEntity<Contract>();
            cc.AddEntity<Order>();
            cc.EnsureTablesCreated();
        }
    }
    class CustomerDBInit : InitializationDbContext { }


}
