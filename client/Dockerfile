FROM node:alpine

WORKDIR /App

COPY . /App

RUN yarn
RUN yarn build

CMD [ "yarn", "run", "develop"] 