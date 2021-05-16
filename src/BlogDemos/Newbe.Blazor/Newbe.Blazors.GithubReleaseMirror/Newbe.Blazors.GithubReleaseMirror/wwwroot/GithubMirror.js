(async () => {
    const listener = details => {
        console.log(details);
        const mirrorUrl = DotNet.invokeMethod('Newbe.Blazors.GithubReleaseMirror', 'GetGithubReleaseMirrorUrl', details.url);
        console.log(`mirror url: ${mirrorUrl}`);
        return {
            redirectUrl: mirrorUrl
        };
    };
    globalThis.browser.webRequest.onBeforeRequest.addListener(listener, {urls: ["*://github.com/*"]}, ["blocking"]);
})();