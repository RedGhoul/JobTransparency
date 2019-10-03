from pytz import utc

from apscheduler.schedulers.background import BlockingScheduler
from apscheduler.jobstores.sqlalchemy import SQLAlchemyJobStore
from apscheduler.executors.pool import ThreadPoolExecutor, ProcessPoolExecutor


jobstores = {
   'default': SQLAlchemyJobStore(url="mysql://APSchedulerJobs:YauaX6pIo9v37w6B@157.245.6.60:3306/APSchedulerJobs?ssl=true",tablename='APSchedulerJobs')
}
executors = {
    'default': ThreadPoolExecutor(20),
    'processpool': ProcessPoolExecutor(5)
}
job_defaults = {
    'coalesce': False,
    'max_instances': 3
}
scheduler = BlockingScheduler(jobstores=jobstores, executors=executors, job_defaults=job_defaults, timezone=utc)

def timed_job():
    print('This job is run every three minutes.')

job = scheduler.add_job(timed_job, 'interval', minutes=2)




scheduler.start()