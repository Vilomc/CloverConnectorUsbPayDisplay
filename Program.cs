using com.clover.remotepay.sdk;
using System;
using System.Threading;

namespace ConsolaUSBConector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cloverConnector = CloverConnectorFactory.CreateUsbConnector(
                "SWDEFOTWBD7XT.9MGLGMDLSYWTV", // remote application ID
                "My POS", // POS name
                "Register 1", // serial number
                true //log aplicacion
            );

            var ccl = new ExampleCloverConnectionListener(cloverConnector); 
            cloverConnector.AddCloverConnectorListener(ccl); 
            cloverConnector.InitializeConnection(); 
            Console.ReadKey();
        }
        public class ExampleCloverConnectionListener : DefaultCloverConnectorListener
        {
            private ICloverConnector cloverConnector;
            public ExampleCloverConnectionListener(ICloverConnector cloverConnector) : base(cloverConnector)
            {
                this.cloverConnector = cloverConnector;
            }
            public override void OnDeviceReady(MerchantInfo merchantInfo) //aqui se ejecuta todas las acciones una vez el dispositivo esta listo para recibir los comandos
            {
                base.OnDeviceReady(merchantInfo);
                Console.WriteLine("Dispositivo disponible");
                // Mostrar mensaje en el dispositivo
                Console.WriteLine("Luego de aqui podemos iniciar acciones");
                //envio de mensajes a la pantalla del dispositivo
                cloverConnector.ShowMessage("Luego de aqui podemos iniciar acciones");
                Thread.Sleep(2000);
                cloverConnector.ShowMessage("Como cambiar el mensaje en pantalla");
                Thread.Sleep(2000);
                cloverConnector.ShowMessage("O para validar los registros en el PDV");
                // Crear la solicitud de venta
                var pendingSale = new SaleRequest();
                pendingSale.ExternalId = ExternalIDUtil.GenerateRandomString(32);
                pendingSale.Amount = 1000;
                Thread.Sleep(5000);
                // Realizar la venta cuando el dispositivo esté listo
                cloverConnector.Sale(pendingSale);
                            }

            public override void OnDeviceConnected()
            {
                base.OnDeviceConnected();
                Console.WriteLine("Dispositivo conectado");
                Console.WriteLine("Aguardamos a que OnDeviceReady responda que el Device esta listo para ejecutar acciones");

            }

            public override void OnDeviceDisconnected()
            {
                base.OnDeviceDisconnected();
                Console.WriteLine("Dispositivo desconectado");
            }

            public override void OnConfirmPaymentRequest(ConfirmPaymentRequest request)
            {
                // Manejar la confirmación del pago si es necesario
            }

            public override void OnSaleResponse(SaleResponse response)
            {   //handler para mostrar resultado de la transaccion
                // Comprobar si la venta fue exitosa y mostrar el resultado
                if (response.Success)
                {
                    Console.WriteLine("\nPago exitoso!\nTransacción ID: " + response.Payment?.externalPaymentId +
                    "\nOrderId: " + response.Payment?.order.id +
                    "\nAmount:  " + response.Payment?.amount +
                    "\nEmployee " + response.Payment?.employee.id +
                    "\nTransactionInfo receiptNumber " + response.Payment?.transactionInfo.receiptNumber +
                    "\nTransactionInfo batchNumber " + response.Payment?.transactionInfo.batchNumber +
                    "\nTransactionInfo installmentsQuantity " + response.Payment?.transactionInfo.installmentsQuantity +
                    "\nTransactionInfo transactionResult " + response.Payment?.transactionInfo.transactionResult +
                    "\nTransactionInfo cardTypeLabel " + response.Payment?.transactionInfo.cardTypeLabel +
                    "\nPayment note " + response.Payment?.note);

                }
                // en caso de error, devolver el motivo
                else
                {
                    Console.WriteLine($"Error en la transacción:\n" +
                    "Result: " + response.Result +
                    "\nReason: "+ response.Reason +
                    "\nMessage: " + response.Message);
                    cloverConnector.ShowMessage("Error en la transacción,\n"+
                    "Reason:"+response.Reason+
                    "\nMessage:"+response.Message);

                }
            }

            public override void OnVerifySignatureRequest(VerifySignatureRequest verifySigRequest)
            {
                // Aceptar o rechazar la firma
                Console.WriteLine("Se requiere verificación de firma...");
                cloverConnector.AcceptSignature(verifySigRequest); // Aquí aceptamos la firma automáticamente

                //de todas formas, los clover hoy en dia se quito esta funcion de chequeo de firma en pantalla.
            }

            public override void OnRetrievePaymentResponse(RetrievePaymentResponse response)
            {
                base.OnRetrievePaymentResponse(response);
            }


        }
    }
}
