const port = Number(process.env.PORT) || 5173;

const server = Bun.serve({
  port,
  async fetch(req) {
    const url = new URL(req.url);
    let path = url.pathname;
    if (path === '/') path = '/index.html';

    const file = Bun.file(`./dist${path}`);
    if (await file.exists()) {
      return new Response(file);
    }

    // SPA fallback to index.html
    const index = Bun.file('./dist/index.html');
    return new Response(index);
  },
});

console.log(`[GalaxyUML React] Server running at http://localhost:${server.port}`);
