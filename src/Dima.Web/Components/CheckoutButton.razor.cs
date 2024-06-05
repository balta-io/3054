using Dima.Core.Handlers;
using Dima.Core.Requests.Orders;
using Dima.Core.Requests.Stripe;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Dima.Web.Components;

public partial class CheckoutButtonComponent : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; } = false;

    #endregion

    #region Services

    [Inject]
    public IStripeHandler StripeHandler { get; set; } = null!;

    [Inject]
    public IOrderHandler OrderHandler { get; set; } = null!;

    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    #endregion

    #region Methods

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;

        var request = new CreateSessionRequest();
        try
        {
            var result = await OrderHandler.CreateOrderAsync(new CreateOrderRequest());
            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível registrar seu pedido", Severity.Error);
                return;
            }

            request.OrderNumber = result.Data?.Number ?? string.Empty;
        }
        catch
        {
            Snackbar.Add("Não foi possível registrar seu pedido", Severity.Error);
        }

        try
        {
            var result = await StripeHandler.CreateSessionAsync(request);
            if (result.Data is not null)
                await JsRuntime.InvokeVoidAsync("checkout", Configuration.StripePublicKey, result.Data);
            else
                Snackbar.Add("Não foi possível iniciar seu pagamento", Severity.Error);
        }
        catch
        {
            Snackbar.Add("Não foi possível iniciar seu pagamento", Severity.Error);
        }
    }

    #endregion
}