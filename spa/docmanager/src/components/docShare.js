import React, { useEffect } from 'react';


const DocShare = ({ share, isEdit, onSaveShare, onDeleteShare }) => {
    const [id] = React.useState(share.id);
    const [username, setUsername] = React.useState(share.username);
    const [permissionSet, setPermissionSet] = React.useState(share.permissionSet);
    const [read, setRead] = React.useState(share?.permissionSet?.includes('R'));
    const [update, setUpdate] = React.useState(share?.permissionSet?.includes('U'));
    const [del, setDel] = React.useState(share?.permissionSet?.includes('D'));
    const [isEmailValid, setIsEmailValid] = React.useState(validateEmail(share.username));

    useEffect(() => {
        let item = { id: id, username: username, permissionSet: permissionSet };
        //console.log('onChange', item);
        setIsEmailValid(validateEmail(username));

        onSaveShare(item)
        console.log('ue-username-permission')
    }, [username, permissionSet]);

    useEffect(() => {
        var state = '';
        if (read) state += 'R';
        if (update) state += 'U';
        if (del) state += 'D';
        setPermissionSet(state);
        console.log('ue-crud', state)
    }, [read, update, del])

    function validateEmail(email) {
        const re = /\S+@\S+\.\S+/;
        return re.test(email);
    }

    function onHoverEnterDelete(e) {
        if (isEdit) e.target.style.color = 'red';
    }

    function onHoverLeaveDelete(e) {
        e.target.style.color = 'lightgrey';
    }

    const getClassNameForEmail = () => {
        if(isEmailValid) return 'c1';
        return 'c1 error';
    }

    return (
        <React.Fragment>
            <input className={isEmailValid ? 'c1' : 'c1 error'} type='text'
                disabled={!isEdit ? 'disabled' : ''}
                defaultValue={share != null ? share.username : ""}
                placeholder= {!isEdit ?'' : 'Email of the user to share with'}
                onChange={e => setUsername(e.target.value)} />

            {/* <input className='c2' type='text' id='permissionSet'
                disabled = {!isEdit ?'disabled' : ''}
                pattern="[A-Za-z]{3}"
                defaultValue={share != null ? share.permissionSet : ""}
                onChange={e => setPermissionSet(e.target.value)} /> */}
            <input type='checkbox'
                className='c2'
                disabled={!isEdit ? 'disabled' : ''}
                defaultChecked={read}
                onChange={() => setRead(current => !current)} />

            <input type='checkbox'
                className='c3'
                disabled={!isEdit ? 'disabled' : ''}
                defaultChecked={update}
                onChange={() => setUpdate(current => !current)} />

            <input type='checkbox'
                className='c4'
                disabled={!isEdit ? 'disabled' : ''}
                defaultChecked={del}
                onChange={() => setDel(current => !current)} />

            {isEdit ?
                <a href='#' className='c5' id={share.id}
                    tag={share.id}
                    style={{ color: 'lightgrey' }}
                    onMouseEnter={onHoverEnterDelete}
                    onMouseLeave={onHoverLeaveDelete}
                    onClick={e => onDeleteShare(share.id)}>Delete</a>
                : <></>
            }
        </React.Fragment>
    );
}

export default DocShare;