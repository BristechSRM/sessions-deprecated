FROM bristechsrm/build_image

COPY . /code 

RUN cd code && mono packages/FAKE/tools/FAKE.exe build.fsx 

EXPOSE 9000

CMD ["mono", "code/build/output/Speakers.exe"]

