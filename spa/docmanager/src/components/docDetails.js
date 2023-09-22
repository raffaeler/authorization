import React, { useEffect } from 'react';
import { useParams } from 'react-router';
import Button from 'react-bootstrap/Button';

import MDEditor from '@uiw/react-md-editor';
import ReactMarkdown from 'react-markdown'

import { useOidc } from "@axa-fr/react-oidc";
import { useOidcIdToken, useOidcAccessToken } from '@axa-fr/react-oidc';

import { GetDocument, PutDocument } from './apiHelper';
import DocShare from './docShare'

const DocDetails = (props) => {
    const { id } = useParams();

    //const { idToken, idTokenPayload } = useOidcIdToken();
    const { accessToken, accessTokenPayload } = useOidcAccessToken();

    const [markdown, setMarkdown] = React.useState("");

    const [fullDocument, setFullDocument] = React.useState({});
    const [document, setDocument] = React.useState({});
    const [shares, setShares] = React.useState([]);
    const [name, setName] = React.useState('');
    const [description, setDescription] = React.useState('');
    const [message, setMessage] = React.useState('');
    const [saved, setSaved] = React.useState(false);
    const [isEdit, setIsEdit] = React.useState(false);

    const [refresh, setRefresh] = React.useState(false);

    useEffect(() => {
        let mounted = true;
        makeGet(id).then(doc => {
            if (mounted) {
                if (doc && doc.document) {
                    //console.log('loaded document: ', doc)
                    setFullDocument(doc);
                    setDocument(doc.document);
                    setShares(doc.document.shares);
                    setName(doc.document.name);
                    setDescription(doc.document.description);
                    setMessage('');
                    setIsEdit(false);
                    setMarkdown(doc.markdown);
                    if (props.mode == 'edit') setIsEdit(true);
                }
            }
        })
        return () => mounted = false;
    }, [refresh]);

    // props.mode == 'edit'
    //console.log("DocDetails with id:", id, "and props", props);

    const makeGet = async (id) => {
        var [isSucceeded, result, errorMessage] = await GetDocument(accessToken, id);

        //console.log("docDetails.makeGet result:", isSucceeded, result, errorMessage);
        if (!isSucceeded) {
            console.log("docDetails.makeGet result:", errorMessage);
            setMessage(errorMessage);
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

    const onEditCancel = () => {
        setRefresh(!refresh);
    }

    const onSave = async () => {
        if (fullDocument?.document == null) {
            setMessage('No document is loaded');
            return;
        }

        fullDocument.document.name = name;
        fullDocument.document.description = description;
        fullDocument.document.shares = shares;
        fullDocument.markdown = markdown;
        //console.log('docDetails.onSave', fullDocument);
        var [isSucceeded, result, errorMessage] = await PutDocument(accessToken, fullDocument);

        //console.log('docDetails.onSave result:', isSucceeded, result, errorMessage);
        if (!isSucceeded) {
            setMessage(errorMessage);
        }
        else {
            setMessage('Saved');
            setIsEdit(false);
        }
    }

    function onHoverEnterDelete(e) {
        if (isEdit) e.target.style.color = 'red';
    }

    function onHoverLeaveDelete(e) {
        e.target.style.color = 'lightgrey';
    }

    const renderShares = () => {
        return (
            <>
                <p></p>
                <h4>Shares</h4>
                <div className='gr5'>
                    <label className='label c1' >Shared with</label>
                    <label className='label c2' >Read</label>
                    <label className='label c3' >Update</label>
                    <label className='label c4' >Delete</label>
                    <label className='label c5' ></label>

                    {
                        shares?.map(element =>
                            <DocShare key={element.id}
                                share={element}
                                isEdit={isEdit}
                                onSaveShare={item => onSaveShare(item)}
                                onDeleteShare={id => onDeleteShare(id)} />)
                    }
                </div>
            </>
        )
    }

    const onSaveShare = (share) => {
        //console.log('onSaveShare', share);
        var index = shares.findIndex(e => e.id === share.id);
        if (index >= 0) {
            shares[index] = share;
            setShares(shares);
            //console.log("share updated: ", share)
        }
    }

    const onAddShare = () => {
        let newShares = [...shares];
        newShares.push({ id: '00000000-0000-0000-0000-000000000000' });
        console.log('onAddShare', newShares);
        setShares(newShares);
    }

    const onDeleteShare = (id) => {
        var index = shares.findIndex(e => e.id === id);
        let newShares = [...shares];
        newShares.splice(index, 1);
        setShares(newShares);
    }

    const debugShares = () => {
        for (const x of document.shares) {
            console.log("share: ", x);
        }
    }

    return (

        <div className='inset'>
            <h2>Document details</h2>

            <div className=''>
                <div className='gr3'>
                    <label className='label c1' >Name:</label>
                    <input className='c2' type='text' id='name'
                        disabled={!isEdit ? 'disabled' : ''}
                        placeholder={!isEdit ? '' : 'Assign a name to this document'}
                        onChange={e => onNameChange(e)}
                        defaultValue={name} />

                    <label className='label c1' >Description:</label>
                    <input className='c2' type='text' id='description'
                        disabled={!isEdit ? 'disabled' : ''}
                        placeholder={!isEdit ? '' : 'Assign a name to this document'}
                        onChange={e => onDescriptionChange(e)}
                        defaultValue={description} />

                    <label className='label c1' >Author:</label>
                    <input className='c2' type='text' id='author'
                        disabled='disabled'
                        defaultValue={document != null ? document.author : ""} />

                </div>
            </div>

            {document?.shares?.length > 0 ? renderShares() : null}

            <div className="bl">
                {!isEdit ?
                    <div style={{ padding: 20, background: 'whitesmoke' }}>
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

            {isEdit ?
                <>
                    <Button variant="primary"
                        className='bl'
                        onClick={onSave}>
                        Save
                    </Button>

                    <Button variant="primary"
                        className='bl b2'
                        onClick={onEditCancel}>
                        Cancel
                    </Button>

                    <Button variant="primary"
                        className='bl b2'
                        onClick={onAddShare}>
                        Add share
                    </Button>

                    {/* <Button variant="primary"
                    className='bl b2'
                    onClick={debugShares}>
                    Dump shares
                </Button> */}
                </>
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