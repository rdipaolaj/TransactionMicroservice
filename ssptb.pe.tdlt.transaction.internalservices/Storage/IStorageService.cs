using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Storage;

namespace ssptb.pe.tdlt.transaction.internalservices.Storage;
public interface IStorageService
{
    Task<ApiResponse<FileUploadResponseDto>> FileUploadAsync(UploadJsonRequestDto request);
}
