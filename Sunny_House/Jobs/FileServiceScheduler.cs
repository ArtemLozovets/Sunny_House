using Quartz;
using Quartz.Impl;
using System.Web;

namespace Sunny_House.Jobs
{
    public class FileServiceScheduler : HttpApplication
    {

        public static void Start()
        {
             ISchedulerFactory schedFact = new StdSchedulerFactory();
             IScheduler scheduler = schedFact.GetScheduler();
            scheduler.Start();


            IJobDetail job = JobBuilder.Create<FileService>().Build();

            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                .StartNow()                             // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x              // настраиваем выполнение действия
                    .WithIntervalInHours(4)             // через 4 часа
                   //.WithIntervalInSeconds(600)             // через 10 sec.
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер

            scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
        }

    }
}