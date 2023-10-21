# Step 1: Build the Angular application in a node environment
FROM node:16 as build-step

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm install

COPY . .

RUN npm run build -- --output-path=./dist/out

# Step 2: Serve the built Angular application using nginx
FROM nginx:alpine

COPY --from=build-step /app/dist/out/ /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]