using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;

namespace Dima.Core.Handlers;

public interface IReportHandler
{
    Task<Response<List<IncomesAndExpenses>?>> GetIncomesAndExpensesReportAsync(GetIncomesAndExpensesRequest request);
    Task<Response<List<IncomesByCategory>?>> GetIncomesByCategoryReportAsync(GetIncomesByCategoryRequest request);
    Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryReportAsync(GetExpensesByCategoryRequest request);
    Task<Response<FinancialSummary?>> GetFinancialSummaryReportAsync(GetFinancialSummaryRequest request);
}