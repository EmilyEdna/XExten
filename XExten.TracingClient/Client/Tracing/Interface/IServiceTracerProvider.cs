namespace XExten.TracingClient.Client.Tracing.Interface
{
    public interface IServiceTracerProvider
    {
        IServiceTracer GetServiceTracer();
    }
}