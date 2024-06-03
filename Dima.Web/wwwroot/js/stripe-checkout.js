window.checkout = (stripePublicKey, sessionId) => {
    let stripe = Stripe(stripePublicKey);
    stripe.redirectToCheckout({
        sessionId: sessionId
    });
}