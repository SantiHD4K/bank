FROM node:20-alpine AS build
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/bank-app /usr/share/nginx/html
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
