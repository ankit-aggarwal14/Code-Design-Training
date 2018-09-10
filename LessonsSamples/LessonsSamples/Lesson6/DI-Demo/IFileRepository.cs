using System.Collections.Generic;

namespace LessonsSamples.Lesson6.Demo
{
    internal interface IFileRepository
    {
        void Add(Movie movie);
    }

    class InMemoryFileRepository : IFileRepository
    {
        private readonly List<Movie> movies = new List<Movie>();

        public void Add(Movie movie)
        {
            movies.Add(movie);
        }
    }
}