# PaymentGateway

Next Steps:
* BFF(Backend For FrontEnd) gatewat to gice the client a single entry point
* IdentiServer - Identity server 4, Client Credential Flow (the client will keep the Secret an the ClientId)
* NorificationProcessor - A service that notify asyncronously the api client readong from a queue in Rabbit MQ

## Test data
at the moment the system allows the client to request card paymente between Card and Merchant. Valid card details are:
* John Doe
```json
{
    "CardNumber":"1111 1111 1111 1111",
    "CardHolderName":"John Doe",
    "MonthExpiryDate":1,
    "YearExpiryDate":1,
    "CVV" = "111"
}
```


* Jane Doe 
```json
{
    "CardNumber":"2222 2222 2222 2222",
    "CardHolderName":"Jane Doe",
    "MonthExpiryDate":2,
    "YearExpiryDate":2   
     CVV = "222"
}
``` 
* Amazon Merchent Id : 53D92C77-3C0E-447C-ABC5-0AF6CF829A22
* Apple Merchant Id :11112C77-3C0E-447C-ABC5-0AF6CF821111

## Test with Postman
Run the solution with docker-compose as statup, open Postaman performa POST to this url 
http://localhost:6000/api/MerchantCardPayments

with this body:
```json
{
    "MerchantId":"53D92C77-3C0E-447C-ABC5-0AF6CF829A22",
    "RequestId":"differenteverycall",
    "CardNumber":"1111 1111 1111 1111",
    "CardHolderName":"John Doe",
    "MonthExpiryDate":1,
    "YearExpiryDate":1,
    "Currency":"GBP",
    "CVV":"111",
    "Amount":10
}
```
This means has been requested a payment from John Doe to Apple of 10 £. You can swap the MerchanId
with the Apple's one or change the card details with the Jane's ones. You will receive a 200 with this body.
```json
{
    "requestId": "differenteverycall",
    "paymentRequestId": "908a22f4-70cc-4f5b-972b-dd265dd0c8f5"
}
```
this emasn the payment has been processed correctly and the reference to retrieve the status is paymentRequestId 
### Get the resutl
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
