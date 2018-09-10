using System;

namespace LessonsSamples.Lesson6.NetCore
{
    class EntityReader : IEntityReader
    {
        private readonly IServiceProvider serviceProvider;

        public EntityReader(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IEntityFieldsReader<T> BeginEntityRead<T>()
        {
            return (IEntityFieldsReader<T>)serviceProvider.GetService(typeof(IEntityFieldsReader<T>));
        }
    }
}