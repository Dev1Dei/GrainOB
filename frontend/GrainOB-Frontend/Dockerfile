# Stage 1: Build and serve the Angular app for development
FROM node:16 AS development
WORKDIR /app
COPY package*.json ./
RUN npm install
RUN npm install -g @angular/cli
COPY . .
CMD ["./node_modules/.bin/ng", "serve", "--host", "0.0.0.0", "--poll", "2000"]

