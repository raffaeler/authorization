import React, {useEffect} from 'react';
import { useParams } from 'react-router';
import Button from 'react-bootstrap/Button';

import MDEditor from '@uiw/react-md-editor';
import ReactMarkdown from 'react-markdown'

import { useOidc } from "@axa-fr/react-oidc";
import { useOidcIdToken, useOidcAccessToken } from '@axa-fr/react-oidc';

import { GetDocument, PutDocument } from './apiHelper';

const DocDetails = (props) => {
    const { id } = useParams();

    //const { idToken, idTokenPayload } = useOidcIdToken();
    const { accessToken, accessTokenPayload } = useOidcAccessToken();

    const [markdown, setMarkdown] = React.useState("## Hello world!!!");

    const [fullDocument, setFullDocument] = React.useState({});
    const [document, setDocument] = React.useState({});
    const [name, setName] = React.useState('');
    const [description, setDescription] = React.useState('');
    const [message, setMessage] = React.useState('');
    const [saved, setSaved] = React.useState(false);
    const [isEdit, setIsEdit] = React.useState(false);

    useEffect(() => {
        let mounted = true;
        makeGet(id).then(doc => {
            if(mounted) {
                if(doc && doc.document) {
                    //console.log('loaded document: ', doc)
                    setFullDocument(doc);
                    setDocument(doc.document);
                    setName(doc.document.name);
                    setDescription(doc.document.description);
                    setMarkdown(doc.markdown);
                    if(props.mode == 'edit') setIsEdit(true);
                }
            }
        })
        return () => mounted = false;
    }, []);

    // props.mode == 'edit'
    //console.log("DocDetails with id:", id, "and props", props);

    const makeGet = async (id) => {
        var [isSucceeded, result, errorMessage] = await GetDocument(accessToken, id);

        //console.log("docDetails.makeGet result:", isSucceeded, result, errorMessage);
        if(!isSucceeded) {
            console.log("docDetails.makeGet result:", errorMessage);
            setMessage(errorMessage.message);
            return [];
        }
        else {
            //console.log("docDetails.makeGet result:", result);
            setMessage('');
            setSaved(true);
            return result;
        }
    }

    function onNameChange(e) {
        e.preventDefault();
        setName(e.target.value);
    }

    function onDescriptionChange(e) {
        e.preventDefault();
        setDescription(e.target.value);
    }

    const onEdit = () => {
        setIsEdit(true);
    }

    const onSave = async () => {
        fullDocument.document.name = name;
        fullDocument.document.description = description;
        fullDocument.markdown = markdown;
        //console.log('docDetails.onSave', fullDocument);
        var [isSucceeded, result, errorMessage] = await PutDocument(accessToken, fullDocument);

        //console.log('docDetails.onSave result:', isSucceeded, result, errorMessage);
        if(!isSucceeded) {
            setMessage(errorMessage);
        }
        else {
            setMessage('Saved');
            setIsEdit(false);
        }
    }

    return (
        
        <div className='inset'>
            <h2>Document details</h2>

            <div className=''>
                <div className='gr'>
                    <label className='c1' >Name:</label>
                    <input className='c2' type='text' id='name'
                        disabled = {!isEdit ?'disabled' : ''}
                        placeholder= {!isEdit ?'' : 'Assign a name to this document'}
                        onChange={e => onNameChange(e)} value={name} />

                    <label className='c1' >Description:</label>
                    <input className='c2' type='text' id='description'
                        disabled = {!isEdit ?'disabled' : ''}
                        placeholder= {!isEdit ?'' : 'Assign a name to this document'}
                        onChange={e => onDescriptionChange(e)} value={description} />

                    <label className='c1' >Author:</label>
                    <input className='c2' type='text' id='author'
                        disabled = 'disabled'
                        value={document != null ? document.author : ""} />
                </div>
            </div>
            

            <div className="bl">
                {!isEdit ? 
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

            {/* <p>{message}</p> */}

            {isEdit ?
                <Button variant="primary"
                    className='bl'
                    onClick={onSave}>
                    Save
                </Button>     
            :
                <Button variant="primary"
                    className='bl'
                    onClick={onEdit}>
                    Edit
                </Button>     

            }
        </div>
    );
}


export default DocDetails;