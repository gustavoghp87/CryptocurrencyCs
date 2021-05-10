const expressApp = require('express')();


expressApp.get("/nodes" , (_, res) => {
    let nodes = ['https://localhost:44354']
    // 'http://localhost:10001', 'http://localhost:10002', 'http://localhost:10003',
    console.log("Proceeding:", nodes);
    res.json(nodes)
})

expressApp.listen(10000, () => {
    console.log("Serving on port 10000");
})
