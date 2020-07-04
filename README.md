# PaymentGateway
## Beta 2 version
### Bug fixed from previous version
* on validation exception now return 400 BadRequest instead of 422 Unprocessable Entity
* removed pitfall  "exception as flow control" in  paymentgateway.api(PG) and paymentgateway.processor.api(PGP)
* improved separation on concerns
* added complete model  validation on PG  (cardnumber,cardholdername, month, year, cvv)
* Improved folder structure (Domain/Entities etc.)
### New features
* Added a client api service that simulates being "amazon" merchant and conduming the payment gateway. 
* Added authentication/authorization using Identity Server 4 with ClientId GrantType. 
* Added sql server as data storage for  paymentgateway.api(PG) and paymentgateway.processor.api(PGP). Used code-first approach and migrations
* Added Prometheus Metrics endpoint for PG and PGP

## Company.IdentityServer (CI)
This service provides authentication and authorization to the PG and PGP services. It supports ClientId grant type,
this means that a client service can authenticate challenging a ClientId and a Secret for an access token.
CI has a static initializer that creates  built-in ApiResouces (PG and PGP) with 1 scope: "CreatePaymentScope".
It provides a Client with simulating an ipothetical "amazon" with "amazonId" and "amazonSecret"
allowed to access the scope "CreatePaymentScope" (basically allowed to process payments)
* This service has the minimum implementation due to merely demonstrare authentication/authorization to PG and PGP

## Client.Payments.Api (CPA)
To extend the payment gateway exercice with authentication we needed a client that whant to consume our PG and PGP.
CPA is a service itself so the flow chosen to authenticate to the CI is the  ClientId flow as CPA is able to store securely 
a ClientId and a Secret, in this case they are in the configuration.  
CPA exposes 1 main apy that allow its company to process payments through PG, in the Payments controller it is simulated 
a merchant card paymetn request towards the PG and the a loop where the payment status is requested to the PGP.
To retrieve the access token has been implementes a TokenProvider that is injected into the 2 proxies (PG proxy and PGP proxy),
in this way the authentication and the functionality are decoupled. 
* This service has the minimum implementation due to merely demonstrare authentication/authorization to PG and PGP 
and how to consume PG and PGP
### Token Provider
As the solition is running in linux containers with docker-compose, the dev certificate to sign the https traffic has not set up 
in the clients at the moment. Therefore has been used an "hack" in the GetAccessToken function that skips the certificate validation error.

## PaymentGateway.Api (PG)
In the startup has been creates a virtual method that configures the authentication with the bearer token(AddJwtBearer). 
This allows to override the function itself during integration tests. The same hack to bypass certification validation
error present in the CI  has been used. 
### Data Storage
A sql server container has been added to the orchestrator to allow data to be persisted. The service uses EF Core with 
code first approach to save Merchants and PaymentsStatuses. A seed migration has been added to have some built in merchants
for testing purposes.

## PaymentGateway.Processor.Api (PGP)
In the startup has been creates a virtual method that configures the authentication with the bearer token(AddJwtBearer). 
This allows to override the function itself during integration tests. The same hack to bypass certification validation
error present in the CI  has been used.
### Data Storage
A sql server container has been added to the orchestrator to allow data to be persisted. The service uses EF Core with 
code first approach to save PaymentsStatuses. 

## Integration Tests
As authentication has been introduces with this release, the pre-existing tests now use the TestStartupNoAuth:Startup class 
to initialize the TestServer. This allows test to bypas security.
*Important
The TestStartupNoAuth has been created in the correspondent service to be tested as it is used to get the 
service directory itself. It also allow the debugger to step into the service code.

## Test data
at the moment the system allows the client to request card paymente between Card and Merchant. Valid card details are:
* John Doe
```json
{
    "CardNumber":"1298 1111 1111 1111",
    "CardHolderName":"John Doe",
    "MonthExpiryDate":1,
    "YearExpiryDate":1,
    "CVV":"111"
}
```


* Jane Doe 
```json
{
    "CardNumber":"1298 2222 2222 2222",
    "CardHolderName":"Jane Doe",
    "MonthExpiryDate":2,
    "YearExpiryDate":2, 
    "CVV":"222"
}
``` 
* Amazon Merchant Id : 53D92C77-3C0E-447C-ABC5-0AF6CF829A22
* Apple  Merchant Id  : 11112C77-3C0E-447C-ABC5-0AF6CF821111

## Test with Postman
Run the solution with docker-compose as statup, open Postaman performa POST to this url 
http://localhost:6000/api/MerchantCardPayments

with this body:
```json
{
    "MerchantId":"53D92C77-3C0E-447C-ABC5-0AF6CF829A22",
    "RequestId":"differenteverycall",
    "CardNumber":"1298 1111 1111 1111",
    "CardHolderName":"John Doe",
    "MonthExpiryDate":1,
    "YearExpiryDate":1,
    "Currency":"GBP",
    "CVV":"111",
    "Amount":10
}
```
This means that a payment has been requested from John Doe to Apple of 10 £. You can swap the MerchanId
with the Apple's one or change the card details with the Jane's ones. You will receive a 200 with this body.
```json
{
    "requestId": "differenteverycall",
    "paymentRequestId": "908a22f4-70cc-4f5b-972b-dd265dd0c8f5"
}
```
this means the payment has been processed correctly and the reference to retrieve the status is paymentRequestId 
### Get the result
To retrieve the result of the transaction (it will be processed asyncronously), with Postaman in anoter tab perform a GET to the following url:

http://localhost:7000/api/PaymentStatuses?paymentId={paymentRequestId}

This call will return a 200 with this body:
```json
{
    "paymentId": "908a22f4-70cc-4f5b-972b-dd265dd0c8f5",
    "requestId": "differenteverycall",
    "status": "Completed",
    "transactionId": "20e1961d-f19c-4065-806e-c0fed4bfebcd",
    "createdAt": "2020-06-25T13:36:14.9262193+00:00",
    "updatedAt": "2020-06-25T13:36:15.0398135+00:00"
}
```
The paymetn completed successfully and the client has the "transactionId" from the bank to reconciliate the payments 

# Tests, mocks
The every service has its own unittest and integration test. The solution has also a Bank.Payments.Api that runs with docker-compose. This project has been added as plus and to test the BankPaymentsProxy in the PaymentGateways.Processor Background worker (PGPB)  when run and debug the solution with F5. The integration tests that involve the PGPB use a mockup class created with Moq that has the same inner logic (same cards and bank accounts allowed to perform transactions.
