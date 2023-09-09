import React, { useState } from 'react';

const acr_to_loa = Object.freeze({
    pwd: 1,
    mfa: 2,
    hwk: 3,
});

const decodeAuthError = async (response) => {
    if (!response.ok) {
        let message;
        try {
            console.log(response);
            var authError = response.headers.get("WWW-Authenticate");
            message = `Fetch failed with HTTP status ${response.status} ${authError}  ${await response.text()}`;
        }
        catch (e) {
            message = `Fetch failed with HTTP status ${response.status} ${response.statusText}`;
        }

        return message;
    }
}

export const PostDocument = async (accessToken, document) => {
    console.log('PostDocument', document);
    if(!accessToken) {
        return [false, null, "Not authenticated"];
    };

    try{
        const options = {
            method: 'POST',
            headers: new Headers({
                //"Authorization": `Bearer ${accessToken}`,
                "Content-Type": "application/json",
                //"Accept": "application/json"
            })
        }
        options.body = JSON.stringify(document);

        const response = await fetch("https://app.iamraf.net:5001/api/documents/", options);
        if(!response.ok) return [false, null, decodeAuthError(response)];

        return [true, await response.json(), null];
    } catch(e) {
        return [false, null, e];
    }
}

export const GetDocuments = async (accessToken) => {
    console.log('GetDocuments');
    if(!accessToken) {
        return [false, null, "Not authenticated"];
    };

    try{
        const options = {
            method: 'GET',
            headers: new Headers({
                //"Authorization": `Bearer ${accessToken}`,
                "Content-Type": "application/json",
                //"Accept": "application/json"
            })
        }
        //options.body = JSON.stringify('');

        const response = await fetch("https://app.iamraf.net:5001/api/documents/", options);
        if(!response.ok) return [false, null, decodeAuthError(response)];

        return [true, await response.json(), null];
    } catch(e) {
        return [false, null, e];
    }
}

export const GetDocument = async (accessToken, id) => {
    console.log('GetDocuments');
    if(!accessToken) {
        return [false, null, "Not authenticated"];
    };

    try{
        const options = {
            method: 'GET',
            headers: new Headers({
                //"Authorization": `Bearer ${accessToken}`,
                "Content-Type": "application/json",
                //"Accept": "application/json"
            })
        }
        //options.body = JSON.stringify('');

        const response = await fetch("https://app.iamraf.net:5001/api/documents/" + id, options);
        if(!response.ok) return [false, null, decodeAuthError(response)];

        return [true, await response.json(), null];
    } catch(e) {
        return [false, null, e];
    }
}

export const PutDocument = async (accessToken, document) => {
    console.log('PutDocument', document);
    if(!accessToken) {
        return [false, null, "Not authenticated"];
    };

    try{
        const options = {
            method: 'PUT',
            headers: new Headers({
                //"Authorization": `Bearer ${accessToken}`,
                "Content-Type": "application/json",
                //"Accept": "application/json"
            })
        }
        options.body = JSON.stringify(document);
        console.log('put id:', document.document.id);
        const response = await fetch("https://app.iamraf.net:5001/api/documents/" + document.document.id, options);
        if(!response.ok) return [false, null, decodeAuthError(response)];

        // PUT does not return any object
        return [true, {}, null];
    } catch(e) {
        return [false, null, e];
    }
}


// export const InvokeAPI = async (resource, requested_loa, previousInvocationOk = true) => {
//     const { idToken, idTokenPayload } = useOidcIdToken();
//     try {
//         console.log(`requesting ${resource} with loa:${requested_loa}`)
//         const token = idToken;
//         //console.log(token);
//         if (!isAuthenticated) {
//             setResult("User is not authenticated");
//             setIsError(true);
//             return;
//         }

//         let token_loa = acr_to_loa[accessTokenPayload.acr];
//         if (token_loa < requested_loa) {
//             setResult("User need higher privileges: " + Object.keys(acr_to_loa)[requested_loa - 1]);
//             setIsError(true);
//             return;
//         }

//         const response = await fetch("https://app.iamraf.net:5001/api/documents/" + resource, {
//             headers: {
//                 Authorization: `Bearer ${token}`,
//             },
//         });

//         if (!response.ok) {
//             let message;
//             try {
//                 console.log(response);
//                 var authError = response.headers.get("WWW-Authenticate");
//                 message = `Fetch failed with HTTP status ${response.status} ${authError}  ${await response.text()}`;
//             }
//             catch (e) {
//                 message = `Fetch failed with HTTP status ${response.status} ${response.statusText}`;
//             }

//             // //if(previousInvocationOk && response.status == 403) {
//             // if(response.status == 403) {
//             //   console.log("fetch access denied: logout+login in progress: " + loa);
//             //   await logout();
//             //   await login(null, {
//             //     acr_values: loa
//             //   });
//             //   invokeAPI(resource, loa, false);
//             //   return;
//             // }

//             setResult(message);
//             setIsError(true);
//             return;
//         }

//         setResult(await response.json());
//         setIsError(false);
//     }
//     catch (e) {
//         console.log(e);
//         setResult(e.message);
//         setIsError(true);
//     }
// }


