const express = require('express');
const expressApp = express();
const fetch = require('node-fetch');
expressApp.use(express.json());
expressApp.use(require('cors')());

let nodes = ['https://localhost:44354', 'http://localhost:10001', 'http://localhost:10002', 'http://localhost:10003'];

expressApp
    .get("/" , (_, res) => {
        res.status(200).json(ok);
    })
    .get("/nodes" , (_, res) => {
        console.log("Sending nodes:", nodes);
        res.json(nodes)
    })
    .post("/registry", async (req, res) => {
        const node = req.body;
        const ip = node.Ip;
        console.log("Registering:", node, ip);
        if (await IsAliveNode(ip)) {
            console.log("Afirmative");
            nodes.push(ip);
            res.status(200).json({ok: "ok 200"});
        }
        else {
            res.status(400);
        }
    })
    .listen(10000, () => {
        console.log("Serving on port 10000");
    })
;

const IsAliveNode = async (node) => {
    const response = await fetch(node, { method: 'GET' } )
    const parsed = await response.json();
    console.log(parsed);
    return true;  
}