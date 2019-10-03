FROM python:3
ENV PYTHONUNBUFFERED 1
RUN mkdir /code
WORKDIR /code
COPY requirements.txt /code/
RUN apt-get update -y
RUN apt-get install python3-pip python3-dev libpq-dev -y
RUN apt-get install unixodbc-dev -y
RUN apt-get install python-pip -y
RUN pip install -r requirements.txt
COPY . /code/

# define the default command to run when starting the container
CMD ["python", "index.py"]