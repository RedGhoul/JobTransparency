from pytz import utc

from apscheduler.schedulers.background import BlockingScheduler
from apscheduler.jobstores.sqlalchemy import SQLAlchemyJobStore
from apscheduler.executors.pool import ThreadPoolExecutor, ProcessPoolExecutor
from JobFinder import startup

jobstores = {
    "default": SQLAlchemyJobStore(
        url="mysql://APSchedulerJobs:YauaX6pIo9v37w6B@157.245.6.60:3306/APSchedulerJobs?ssl=true",
        tablename="APSchedulerJobs",
    )
}
executors = {"default": ThreadPoolExecutor(20), "processpool": ProcessPoolExecutor(5)}
job_defaults = {"coalesce": False, "max_instances": 3}


def jobfinder_activate():
    print("jobfinder_activated")
    startup()


def iamAlive():
    print("I am Alive")


if __name__ == "__main__":
    scheduler = BlockingScheduler(
        jobstores=jobstores,
        executors=executors,
        job_defaults=job_defaults,
        timezone=utc,
    )

    jobiamAlive = scheduler.add_job(iamAlive, trigger="cron", second="*")

    # jobjobfinder_activate = scheduler.add_job(
    #     jobfinder_activate, trigger="cron", minute="*/5"
    # )

    jobjobfinder_activate = scheduler.add_job(
        jobfinder_activate, trigger="cron", hour="22", minute="30"
    )

    scheduler.start()
