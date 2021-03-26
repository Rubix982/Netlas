FROM node

WORKDIR /App

COPY ./package.json /App/

COPY ./yarn.lock /App/

RUN yarn

COPY ./ /App/

CMD ["yarn","start"]