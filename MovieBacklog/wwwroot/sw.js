const CACHE_NAME = "movie-backlog-static-v5";
const STATIC_ASSETS = [
    "/offline.html",
    "/css/site.css",
    "/lib/bootstrap/dist/css/bootstrap.min.css",
    "/icons/logo-32x32.png",
    "/icons/logo-512x512.png"
];

self.addEventListener("install", event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(STATIC_ASSETS))
            .then(() => self.skipWaiting())
    );
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys()
            .then(cacheNames => Promise.all(
                cacheNames
                    .filter(cacheName => cacheName.startsWith("movie-backlog-") && cacheName !== CACHE_NAME)
                    .map(cacheName => caches.delete(cacheName))
            ))
            .then(() => self.clients.claim())
    );
});

self.addEventListener("fetch", event => {
    const request = event.request;
    const requestUrl = new URL(request.url);

    if (request.method !== "GET" || requestUrl.origin !== self.location.origin) {
        return;
    }

    if (request.mode === "navigate") {
        event.respondWith(
            fetch(request).catch(() => caches.match("/offline.html"))
        );
        return;
    }

    if (STATIC_ASSETS.includes(requestUrl.pathname)) {
        event.respondWith(
            caches.match(request).then(cachedResponse => {
                if (cachedResponse) {
                    return cachedResponse;
                }

                return fetch(request).then(response => {
                    if (response.ok) {
                        const responseToCache = response.clone();
                        caches.open(CACHE_NAME).then(cache => cache.put(request, responseToCache));
                    }
                    return response;
                });
            })
        );
    }
});
