﻿using Unity;
using Unity.Lifetime;

namespace LessonsSamples.Lesson6
{
	static class UnityDemo
	{
		public static void MainFunc()
		{
			UnityContainer container = new UnityContainer();

			container.RegisterType<ICommand, MovieConsoleCreator>(nameof(MovieConsoleCreator));
			container.RegisterType<ICommand, MovieTranslator>(nameof(MovieTranslator));

			// Demo the difference between PerResolveLivetimeManager and TransientLifetimeManager
			container.RegisterType<ITextStorage, FileStorage>(new TransientLifetimeManager()); 


			var app = container.Resolve<MovieConsoleApplication>();
			app.Run();
		}
	}
}