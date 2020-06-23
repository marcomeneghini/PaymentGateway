BANK - Validate request Model
BANK - Change Validation error Response (422 status code)
BANK - Make API Idempotent (return  409 Conflict)
BANK - Add Healthcheck 
BANK - Run in container
SharedLib  - Add Encryption Service
PG_API - Encrypt payload (IDataProtecionProvider??)
PG_API - Create MerchantCardPaymentsController  (This API will be available to the Merchant)
PG_API - CreatePayment POST must be idempotent (see BANK)
PG_API - Add Healthcheck
PG_API - CreatePayment Retrieve the merchant, if does not exists or IsValid=false, InvalidMerchantException
PG_API - UnitTesting PaymentService
SHRD - EncryptedMessage
SHRD - IEventBrokerPublisher PublishEncryptedMessage
SHRD - RabbitMqEventBrokerPublisher:IEventBrokerPublisher
DOCKER - Compose , Add RabbitMQ
PG_API - Publish message "PaymentRequestMessage" to "PaymentRequests" Exchange
SHRD - IEventBrokerSubscriber Subscribe(topicname)
SHRD - IEventBrokerSubscriber MessageArrived()
PG_PROC - Listen Queue PaymentRequests
PG_PROC - finalize service

------MVP 1 COMPLETE!(some Extras included already) -----------

BANK - Change Enum To String in the model for status
PG_PROC - implement Polly on Bank Proxy
PG_PROC - proxy and consumer throw correct exceptions
-- DONE -
PG_PROC - implement ExceptionMiddleware (DEPRECATED - the work is don in the background worker)
PG_PROC - add CreatedAt and LAstUpdate to PaymentStatus
-- DONE
PG_API - ADD SERILOG 
PG_API - ADD ErrorHandlind Middleware
PG_API - ADD ErrorResponseModel
-- DONE
-- PG_PROC - DEbug PaymentStatus
-- DONE
SHRD_UT - EncryptedMessage and Cipher functional tests
-- DONE
BANK	- AddTransactionId
PG_PROC - AddTransactionId
-- DONE
PG_PROC - Test ChannelConsumer
PG_PROC - Test ChannelProducer
-- DONE
PG_API  - IMPROVE LOGS
--DONE
PG_PROC - SERILOG - IMPROVE LOGS
--DONE
------MVP 2 COMPLETE! -----------

I think it could be enough for a test, otherwise i'd start thinking to commercilize it :D

-----------------------
EXTRAS
-----------------------
DONE:
Containerization 
Encryption 
Bank Implementation
Application logging = SERILOG  
Exception Handling Middleware
Unit Testing

------ OUTSTANDING -------------
ADD Application metrics 
ADD Authentication 
ADD Build script / CI 
ADD Performance testing = Gatling
ADD Data storage 
------------------------
EXTRA EXTRA:
ADD BFF CLIENT GATEWAY 
ADD NOtification.Api (to notify the client)
-----------------------



