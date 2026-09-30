using ModelService.Domain;

namespace ModelService.Application;

public interface IFacetInferenceEngine
{
    Task<FacetInferenceBatch> InferAsync(
        string answer,
        IReadOnlyList<string> claims,
        CancellationToken cancellationToken);
}

/// <summary>推理负载指标，供 /readyz 与运维观测。</summary>
public interface IInferenceLoadStats
{
    int InflightBatches { get; }
    long CompletedBatches { get; }
}

public interface IModelAssetStatusReader
{
    IReadOnlyList<ModelAssetState> Inspect();
}
