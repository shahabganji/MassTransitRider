using Pulumi;

using Pulumi.AzureNative.Resources;

namespace MassTransitRider.Provisioning;

internal class MyStack : Stack
{
    public MyStack()
    {
        var resourceGroup = new ResourceGroup("rg-mt-riders", new ResourceGroupArgs
        {
            ResourceGroupName = "rg-mt-riders"
        });
        
        _ = new Pulumi.AzureNative.Storage.StorageAccount("st-mt-riders", 
            new Pulumi.AzureNative.Storage.StorageAccountArgs
            {
                Kind = "StorageV2",
                AccountName = "stmtriders",
                Sku = new Pulumi.AzureNative.Storage.Inputs.SkuArgs
                {
                    Name = "Standard_LRS",
                },
                ResourceGroupName = resourceGroup.Name,
                EnableHttpsTrafficOnly = true,
            });

        _ = new Pulumi.AzureNative.ServiceBus.Namespace("sb-mt-riders",
            new Pulumi.AzureNative.ServiceBus.NamespaceArgs
            {
                ResourceGroupName = resourceGroup.Name,
                NamespaceName = "sb-mt-riders",
            });
        
        var eventHubNamespace = new Pulumi.AzureNative.EventHub.Namespace("evhns-mt-riders",
            new Pulumi.AzureNative.EventHub.NamespaceArgs()
            {
                NamespaceName = "evhns-mt-riders",
                ResourceGroupName = resourceGroup.Name
            });

        _ = new Pulumi.AzureNative.EventHub.EventHub("evh-riders", new Pulumi.AzureNative.EventHub.EventHubArgs
        {
            ResourceGroupName = resourceGroup.Name,
            NamespaceName = eventHubNamespace.Name,
            EventHubName = "evh-riders"
        });
    }
}
