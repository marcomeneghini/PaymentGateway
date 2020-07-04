* BANK - Validate request Model
* BANK - Change Validation error Response (422 status code)
* BANK - Make API Idempotent (return  409 Conflict)
* BANK - Add Healthcheck 
* BANK - Run in container
* SharedLib  - Add Encryption Service
* PG_API - Encrypt payload (IDataProtecionProvider??)
* PG_API - Create MerchantCardPaymentsController  (This API will be available to the Merchant)
* PG_API - CreatePayment POST must be idempotent (see BANK)
* PG_API - Add Healthcheck
* PG_API - CreatePayment Retrieve the merchant, if does not exists or IsValid=false, InvalidMerchantException
* PG_API - UnitTesting PaymentService
* SHRD - EncryptedMessage
* SHRD - IEventBrokerPublisher PublishEncryptedMessage
* SHRD - RabbitMqEventBrokerPublisher:IEventBrokerPublisher
* DOCKER - Compose , Add RabbitMQ
* * PG_API - Publish message "PaymentRequestMessage" to "PaymentRequests" Exchange
* SHRD - IEventBrokerSubscriber Subscribe(topicname)
* SHRD - IEventBrokerSubscriber MessageArrived()
* PG_PROC - Listen Queue PaymentRequests
* PG_PROC - finalize service

------MVP 1 COMPLETED!(some Extras included already) -----------

* BANK - Change Enum To String in the model for status
* PG_PROC - implement Polly on Bank Proxy
* PG_PROC - proxy and consumer throw correct exceptions
* -- DONE -
* PG_PROC - implement ExceptionMiddleware (DEPRECATED - the work is don in the background worker)
* PG_PROC - add CreatedAt and LAstUpdate to PaymentStatus
* -- DONE
* PG_API - ADD SERILOG 
* * * PG_API - ADD ErrorHandlind Middleware
* PG_API - ADD ErrorResponseModel
* -- DONE
* -- PG_PROC - DEbug PaymentStatus
* -- DONE
* SHRD_UT - EncryptedMessage and Cipher functional tests
* -- DONE
* BANK	- AddTransactionId
* PG_PROC - AddTransactionId
* -- DONE
* PG_PROC - Test ChannelConsumer
* PG_PROC - Test ChannelProducer
* -- DONE
* PG_API  - IMPROVE LOGS
* --DONE
* PG_PROC - SERILOG - IMPROVE LOGS
* --DONE
* ------MVP 2 COMPLETED! -----------
* 
* PG_API - INTEGRATION TEST - OK
* PG_PROC - INTEGRATION TEST GetPaymentStatus Not Found- OK
* PG_PROC - INTEGRATION TEST GetPaymentStatus Completed- OK
* PG_PROC - INTEGRATION TEST GetPaymentStatus Scheduled- OK
* PG_PROC - INTEGRATION TEST GetPaymentStatus Error - OK

* PG_PROC - INTEGRATION - Inject Mock<IBankPaymentRepository> 
* 
* E2E Integration Tests - 2 services+RabbitMQ in container
* 
* ------Beta 1 COMPLETED! -----------
* -----------------------------------
* --------- Oustanding 24/6 ---------
* -----------------------------------
* PG_API	Swagger + ProducesResponseType -- OK
* PG_PROC Swagger + ProducesResponseType -- OK

* PG_API	Verify Validation (422) -- OK
* PG_PROC Verify Validation (422) -- OK

* Cleanup
* -----------------------------------
* BUG - Adding the ValidationAttribute to the Controllrt
 * make disapper the Swagger Controller :O
* -----------------------------------
* 
* -----------------------
* * EXTRAS
* -----------------------
* DONE:
* Containerization 
* Encryption 
* Bank Implementation
* Application logging = SERILOG  
* Exception Handling Middleware
* Unit Testing

## ------ OUTSTANDING -------------
* ADD Application metrics 
* ADD Authentication 
* ADD Client API
* ADD Build script / CI 
* ADD Performance testing = Gatling
* ADD Data storage 
* ------------------------
* EXTRA EXTRA:
* ADD BFF CLIENT GATEWAY 
* ADD NOtification.Api (to notify the client)
* ------------------------------------

## ------------ 30/06/2020 ------------
* Bugfixing : exception as flow control						OK
* -- PG_API - removed exception as flow control				OK
* Bugfixing : separation of concerns						OK
* Bugfixing : wrong http code (422 Unprocessable Entity)	OK

* PG_PROC - CardPaymentRequest/Response into domain entities (CardPayment) OK
* PG_PROC - removed exception as flow control BankPaymentProxy / PaymentResult add an error code 
* Check TODOs in code													OK
* PG_API Add Validators (sort code, account number, month, year, name) OK 
* PG_API ValidationResultModel:ErrorResponseModel	and add err code  OK
* PG_PROC ValidationResultModel:ErrorResponseModel	and add err code OK

## --------------------------- New branches ---------------------------
### Add API Client
* Added PaymentsController that cretes the payment and iterates n times 
to call get the payment status. IT retirns an aggregate set of informations.  OK
* Add serilog OK
* Implement the PaymentStatsesController OK
* Add swagger OK 
* Add RequestId to the RequestBody  OK

### Add Authentication
* add identity server		OK
* add PG_API as resource	OK
* add PR_PROC as resource	OK
* fix tests					OK
### Add Data Storage
* PG_API	add DB			OK
* PG_PROC	add DB			OK
* PG_PROC	fix tests		OK

### Add Application Metric 


## ------final review
* 
* Add LogEntry for structured log (in sharedlib)
* PG_API	- Add LogEntry 
* PG_PROC	- Add LogEntry
* General	- Add logs visulizer 
* CLI - Check error flow

