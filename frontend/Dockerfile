FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

# C5 fix: Accept VITE_API_URL as a build argument — Vite bakes env vars into the
# static bundle at build time. Without this the browser would never see the value.
ARG VITE_API_URL=http://localhost:5121/api
ENV VITE_API_URL=$VITE_API_URL

RUN npm run build

# ─── Production stage (serve static files via a lightweight server) ───────────
FROM node:20-alpine

WORKDIR /app

RUN npm install -g serve

COPY --from=build /app/dist ./dist

EXPOSE 5173

CMD ["serve", "-s", "dist", "-l", "5173"]