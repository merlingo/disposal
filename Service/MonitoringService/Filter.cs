

namespace MonitoringService
{
    public abstract class Filter
    {
        public abstract Chunk filter(Chunk datasource, EventBus eb);
    }
}
