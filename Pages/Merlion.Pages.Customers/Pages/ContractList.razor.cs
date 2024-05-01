using Merlion.Base;
using Merlion.Component.Dialogs;
using Merlion.Component.Inners;
using Merlion.Pages.Customers.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MudBlazor;
using System.Globalization;
using System.Reflection;

namespace Merlion.Pages.Customers.Pages
{
    public partial class ContractList : ComponentBase
    {
        public List<VisualContract> Items { get; set; }
        public Dictionary<string, string> Customers { get; set; }
        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            try
            {
                Items = (from contract in context.Set<Contract>()
                         from customer in context.Set<Customer>()
                         where contract.CustomerCode == customer.Code
                         select new VisualContract()
                         {
                             Code = contract.Code,
                             Name = contract.Name,
                             CustomerCode = contract.CustomerCode,
                             CustomerName = customer.Name,
                             ContentInBrief = contract.ContentInBrief,
                             IsClosed = contract.IsClosed,
                             SigningDate = contract.SigningDate.Value.ToDateTime(TimeOnly.MinValue)
                         }).ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            var ctmers = context.Set<Customer>().Select(x => new { Code = x.Code, Name = x.Name }).OrderBy(x => x.Name).ToList();
            Customers = new Dictionary<string, string>();
            foreach (var customer in ctmers)
            {
                Customers.Add(customer.Code, customer.Name);
            }
        }
        private async Task SaveContract(JointContext<VisualContract> jtContext)
        {
            using var context = await _pdbFactory.CreateDbContextAsync(this);
            switch (jtContext.OperationType)
            {
                case "Edit":
                    var item = context.Set<Contract>().FirstOrDefault(x => x.Code == jtContext.DataModel!.Code);
                    if (item != null)
                    {
                        //item.Code = jtContext.DataModel!.Code;
                        item.Name = jtContext.DataModel!.Name;
                        item.CustomerCode = jtContext.DataModel!.CustomerCode;
                        item.ContentInBrief = jtContext.DataModel!.ContentInBrief;
                        item.SigningDate = DateOnly.FromDateTime(jtContext.DataModel!.SigningDate.Value);
                        item.IsClosed = jtContext.DataModel!.IsClosed;
                        await context.SaveChangesWithAuditAsync();
                        var record = Items.FirstOrDefault(x => x.Code == jtContext.DataModel.Code);
                        if (record is not null)
                        {
                            //failed, it's a new obj
                            //record = Utilities.ForceCopyToSubInstance<Customer, VisualCustomer>(item);
                            record.Name = item.Name;
                            record.CustomerCode = item.CustomerCode;
                            record.CustomerName = Customers[item.CustomerCode];
                            record.ContentInBrief = item.ContentInBrief;
                            record.SigningDate = item.SigningDate.Value.ToDateTime(TimeOnly.MinValue);
                            record.IsClosed = item.IsClosed;

                        }
                    }
                    break;
                case "New":
                    Contract newItem = new Contract();
                    newItem.Code = jtContext.DataModel!.Code;
                    newItem.Name = jtContext.DataModel!.Name;
                    newItem.CustomerCode = jtContext.DataModel!.CustomerCode;
                    newItem.ContentInBrief = jtContext.DataModel!.ContentInBrief;
                    newItem.SigningDate = DateOnly.FromDateTime(jtContext.DataModel!.SigningDate.Value);
                    newItem.IsClosed = jtContext.DataModel!.IsClosed;

                    context.Set<Contract>().Add(newItem);
                    await context.SaveChangesWithAuditAsync();
                    VisualContract newRecord = new VisualContract();
                    Utilities.CopyPropertiesFrom<VisualContract, Contract>(newRecord, newItem);
                    //var newRecord = Utilities.ForceCopyToSubInstance<Contract, VisualContract>(newItem);
                    newRecord.CustomerName = Customers[jtContext.DataModel!.CustomerCode];
                    Items.Add(newRecord);
                    break;
            }
            _snackbarService.Add(Loc["The operation is completed successfully!"], Severity.Success);

            jtContext.CloseDialog();
        }

        private async Task DeleteContract(JointContext<VisualContract> jtContext)
        {

            await _dialogService.ShowConfirmDialog(Loc["Delete"], Loc["Are you aure to delete"], null, jtContext.DataModel.Code,
                                   async (CommonDialogEventArgs e) =>
                                   {
                                       using var context = await _pdbFactory.CreateDbContextAsync(this);
                                       bool exist = context.Set<Order>().Any(x => x.ContractCode == jtContext.DataModel.Code);
                                       if (exist)
                                       {
                                           this._snackbarService.Add(Loc["Some orders are still running under this contract, the operation is cancelled!"], Severity.Error);
                                       }
                                       else
                                       {
                                           var contract = context.Set<Contract>().FirstOrDefault(x=>x.Code == jtContext.DataModel.Code);
                                           if (contract != null)
                                           {
                                               context.Remove<Contract>(contract);
                                               await context.SaveChangesWithAuditAsync();
                                               Items.RemoveAll(x => x.Code == jtContext.DataModel.Code);
                                               this._snackbarService.Add(Loc["This data has been removed successfully!"], Severity.Success);
                                           }
                                           else
                                           {
                                               this._snackbarService.Add(Loc["This data cannot be found, the operation is cancelled!"], Severity.Error);
                                           }
                                       }
                                       //StateHasChanged();
                                   });
        }
     }
}
