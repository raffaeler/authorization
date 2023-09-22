import React from 'react';
import Button from 'react-bootstrap/Button';

import MDEditor from '@uiw/react-md-editor';
import ReactMarkdown from 'react-markdown'

import { useOidc } from "@axa-fr/react-oidc";
import { useOidcIdToken, useOidcAccessToken } from '@axa-fr/react-oidc';

import { PostDocument } from './apiHelper';

const DocCreate = () => {
    //const { idToken, idTokenPayload } = useOidcIdToken();
    const { accessToken, accessTokenPayload } = useOidcAccessToken();

    const [markdown, setMarkdown] = React.useState("");

    const [name, setName] = React.useState('');
    const [description, setDescription] = React.useState('');
    const [message, setMessage] = React.useState('');
    const [saved, setSaved] = React.useState(false);

    function onNameChange(e) {
        e.preventDefault();
        setName(e.target.value);
    }

    function onDescriptionChange(e) {
        e.preventDefault();
        setDescription(e.target.value);
    }

    const onSave = async () => {
        var [isSucceeded, result, errorMessage] = await PostDocument(accessToken, {
            document: {
                name: name,
                description: description,
            },
            markdown: markdown
        });

        //console.log("docCreate.onSave result: ", isSucceeded, result, errorMessage);
        if(!isSucceeded) {
            setMessage(errorMessage);
        }
        else {
            setMessage('Saved');
            setSaved(true);
        }
    }

    return (
        
        <div className='inset'>
            <h2>Create a new document</h2>

            <div className=''>
                <div className='gr'>
                    <label className='c1' >Name:</label>
                    <input className='c2' type='text' id='name'
                        disabled = {saved ?'disabled' : ''}
                        placeholder= {saved ?'' : 'Assign a name to this document'}
                        onChange={e => onNameChange(e)} Value={name} />

                    <label className='c1' >Description:</label>
                    <input className='c2' type='text' id='description'
                        disabled = {saved ?'disabled' : ''}
                        placeholder= {saved ?'' : 'Assign a name to this document'}
                        onChange={e => onDescriptionChange(e)} Value={description} />
                </div>
            </div>
            

            <div className="bl">
                {saved ? 
                    <div style={{padding:20, background:'whitesmoke'}}>
                        <ReactMarkdown children={markdown} />
                    </div>
                    : <>
                    <MDEditor
                        value={markdown}
                        onChange={setMarkdown}
                    />
                    {/* <MDEditor.Markdown source={markdown} style={{ whiteSpace: 'pre-wrap' }} /> */}
                    </>
                }
            </div>

            <p>{message}</p>

            <Button variant="primary"
                className='bl'
                onClick={onSave}
                disabled = {saved ?'disabled' : ''} >
                Save
            </Button>
        </div>
    );
}


export default DocCreate;