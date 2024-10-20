using Mapster;
using ssptb.pe.tdlt.transaction.dto.Blockchain;
using ssptb.pe.tdlt.transaction.dto.Transaction;
using ssptb.pe.tdlt.transaction.entities;
using ssptb.pe.tdlt.transaction.entities.Enums;

namespace ssptb.pe.tdlt.transaction.dto.Mapster;
public static class MapsterConfiguration
{
    public static TypeAdapterConfig Configuration()
    {
        TypeAdapterConfig config = new();

        config.NewConfig<TransactionRequestDto, entities.Transaction>()
            .Map(dest => dest.Tag, src => src.Tag)
            .Map(dest => dest.BankTransactionId, src => src.BankTransactionId)
            .Map(dest => dest.TransactionData, src => src.TransactionData)
            .Map(dest => dest.TransactionDataSave, src => src.TransactionData.GetRawText())
            .Map(dest => dest.TransactionDate, src => src.TransactionDate)
            .Map(dest => dest.Status, src => TransactionStatus.SentToBlockchain);

        config.NewConfig<NodeInfoDto, NodeStatusDto>()
           .Map(dest => dest.IsHealthy, src => src.NodeInfo.Status.IsHealthy)
            .Map(dest => dest.Version, src => src.NodeInfo.Version)
            .Map(dest => dest.NetworkName, src => src.NodeInfo.Protocol.NetworkName);

        return config;
    }
}
