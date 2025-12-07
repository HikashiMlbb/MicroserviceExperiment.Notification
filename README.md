## API Health Check:
### `GET /healthz`

---

## Fill .env for Application:

```.env
RABBITMQ__CONNECTION_STRING = "* Connection string for RabbitMQ *"
RABBITMQ__QUEUE_NAME = "* RabbitMQ Queue Name for Notifying *"
SMTP__EMAIL_NAME = "* Name from which the email letter will be sent *"
SMTP__EMAIL_ADDRESS = "* Email Address from which the email letter will be sent*"
SMTP__HOST = "* Host of SMTP Server *"
SMTP__PORT = "* Port of SMTP Server *"
SMTP__USE_SSL = "* Is SSL Used? (true/false) *"
SMTP__LOGIN = "* Login for SMTP Server *"
SMTP__PASSWORD = "* Password for SMTP Server *"
```
