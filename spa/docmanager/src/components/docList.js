import React, {useEffect} from 'react';
import Button from 'react-bootstrap/Button';

import { useOidc } from "@axa-fr/react-oidc";
import { useOidcIdToken, useOidcAccessToken } from '@axa-fr/react-oidc';

import { LinkContainer } from "react-router-bootstrap";
import DocDetails from './docDetails';

import { GetDocuments } from './apiHelper';

const DocList = () => {
    //const { idToken, idTokenPayload } = useOidcIdToken();
    const { accessToken, accessTokenPayload } = useOidcAccessToken();

    const [markdown, setMarkdown] = React.useState("## Hello world!!!");

    const [documents, setDocuments] = React.useState([]);

    const [name, setName] = React.useState('');
    const [description, setDescription] = React.useState('');
    const [message, setMessage] = React.useState('');

    useEffect(() => {
        let mounted = true;
        makeGet().then(docs => {
            if(mounted) {
                setDocuments(docs);
            }
        })
        return () => mounted = false;
    }, []);

    const makeGet = async () => {
        var [isSucceeded, result, errorMessage] = await GetDocuments(accessToken);

        console.log("result: ", isSucceeded, result, errorMessage);
        if(!isSucceeded) {
            console.log(errorMessage);
            setMessage(errorMessage.message);
            return [];
        }
        else {
            setMessage('');
            return result;
        }
    }

    const renderTemplate = (item) => {
        return(
            <>
            <LinkContainer to={'/details/' + item.document.id} className='c1' >
                <a href='#' id='name'>{item.document.name}</a>
            </LinkContainer>
            <div className='c2' id='description'>{item.document.description}</div>
            <div className='c3' id='author'>{item.document.author}</div>
            <div className='c4' id='actions'>
                LCRUD
            </div>
            </>
        )
    }

    return (
        <div className='inset'>
            <h2>Available documents</h2>

            <div className=''>
                <div className='gr'>
                    
                    <label className='c1' for='name'>Name</label>
                    <label className='c2' for='description'>Description</label>
                    <label className='c3' for='author'>Author</label>
                    <label className='c4' for='actions'>Actions</label>

                {documents.map(element => renderTemplate(element))}

                </div>
            </div>
            

            <p><pre>{message}</pre></p>

        </div>
    );
}


export default DocList;