import React, { useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Stack from 'react-bootstrap/Stack';

import { useOidc } from "@axa-fr/react-oidc";
import { useOidcIdToken, useOidcAccessToken } from '@axa-fr/react-oidc';

import { LinkContainer } from "react-router-bootstrap";
import DocDetails from './docDetails';

import { GetDocuments, DeleteDocument } from './apiHelper';

const DocList = () => {
    //const { idToken, idTokenPayload } = useOidcIdToken();
    const { accessToken, accessTokenPayload } = useOidcAccessToken();

    const [markdown, setMarkdown] = React.useState("## Hello world!!!");

    const [documents, setDocuments] = React.useState([]);

    const [name, setName] = React.useState('');
    const [description, setDescription] = React.useState('');
    const [message, setMessage] = React.useState('');
    const [refresh, setRefresh] = React.useState(false);

    useEffect(() => {
        let mounted = true;
        makeGet().then(docs => {
            if (mounted) {
                setDocuments(docs);
            }
        })
        return () => mounted = false;
    }, [refresh]);

    const makeGet = async () => {
        var [isSucceeded, result, errorMessage] = await GetDocuments(accessToken);

        console.log("docList.makeGet result: ", isSucceeded, result, errorMessage);
        if (!isSucceeded) {
            console.log('docList.makeGet error:', errorMessage);
            setMessage(errorMessage.message);
            return [];
        }
        else {
            setMessage('');
            return result;
        }
    }

    async function onDelete(e) {
        //console.log('docList.onDelete', e.target.id);
        await DeleteDocument(accessToken, e.target.id);
        //console.log('docList.makeGet', e.target.id, 'deleted');
        setRefresh(!refresh);
    }

    function onHoverEnterDelete(e) {
        e.target.style.color = 'red';
    }

    function onHoverLeaveDelete(e) {
        e.target.style.color = 'lightgrey';
    }

    const renderActions = (item) => {
        //console.log('docList.renderActions', item.document.id);
        return (
            <a href='#' id={item.document.id}
                tag={item.document.id}
                style={{color: 'lightgrey'}}
                onMouseEnter={onHoverEnterDelete}
                onMouseLeave={onHoverLeaveDelete}
                onClick={e => onDelete(e)}>Delete</a>
        );
    }

    // </> is a shortcut for <React.Fragment />
    const renderTemplate = (item) => {
        return (
            <React.Fragment key={item.document.id}>
                <LinkContainer to={'/details/' + item.document.id} className='c1'>
                    <a href='#' id='name'>{item.document.name}</a>
                </LinkContainer>
                <div className='c2' id='description'>{item.document.description}</div>
                <div className='c3' id='author'>{item.document.author}</div>
                <div className='c4' id='actions'>
                    {renderActions(item)}
                </div>
            </React.Fragment>
        )
    }

    return (
        <div className='inset'>
            <h2>Available documents</h2>

            <div className=''>
                <div className='gr'>
                    <label className='c1' >Name</label>
                    <label className='c2' >Description</label>
                    <label className='c3' >Author</label>
                    <label className='c4' >Actions</label>


                    {documents.map(element => renderTemplate(element))}

                </div>
            </div>


            <pre>{message}</pre>

        </div>
    );
}


export default DocList;