import React from 'react';
import './loginControl.css';
import { useOidc } from "@axa-fr/react-oidc";
import { useOidcUser } from '@axa-fr/react-oidc';

import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';

function LoginControl(props) {
    const { login, logout, isAuthenticated} = useOidc();
    const { oidcUser, oidcUserLoadingState } = useOidcUser();

    // const removeSessionStorageOidc = () => {
    //     var oidcKeys = Object.keys(sessionStorage)
    //         .filter((key) => key.startsWith('oidc.user'));
    //     //console.log(oidcKeys);
    //     oidcKeys.forEach(k => sessionStorage.removeItem(k));
    // }

    const loginPlain = async () => {
        await login();
    }

    const loginMfa = async () => {
        await login(null, {
            acr_values: "mfa"
        });
        // await auth.signinRedirect({
        //     acr_values: "mfa"
        // });
    }

    const loginHwk = async () => {
        await login(null, {
            acr_values: "hwk"
        });

        // await auth.signinRedirect({
        //     acr_values: "hwk"
        // });
    }

    const logoutPlain = async () => {
        await logout();
        //await auth.removeUser();
        props.onLogout();
    }

    const logoutAndRevoke = async () => {
        await logout();
        //await auth.revokeTokens(["access_token", "refresh_token"]);
        //await auth.removeUser();
        //removeSessionStorageOidc();
        props.onLogout();
    }

    // if (auth.isLoading) {
    //     return <div>Loading...</div>;
    // }

    // if (auth.error) {
    //     return <div>Authentication error: {auth.error.message}</div>;
    // }
    if (isAuthenticated) {
        //console.log(oidcUser);
        let name = oidcUser == null ? "(none)" : oidcUser.name; 
        return (
            <NavDropdown title={'Hello ' + name} id="basic-nav-dropdown" >
                <NavDropdown.Item href="#" onClick={logoutPlain}>Log out</NavDropdown.Item>
                <NavDropdown.Item href="#" onClick={logoutAndRevoke}>Log out and Revoke</NavDropdown.Item>
            </NavDropdown>

            // <div className="dropdown">
            //     <button className="dropbtn">Hello {name}</button>
            //     <div className="dropdown-content">
            //         <a href="#" onClick={logoutPlain}>Log out</a>
            //         <a href="#" onClick={logoutAndRevoke}>Log out and Revoke</a>
            //     </div>
            // </div>
            //    {/* <div>Claim sub: {auth.user.profile['sub']}</div> */}
        );
    }
    else {
        return (
            <NavDropdown title="Login" id="basic-nav-dropdown" >
                <NavDropdown.Item href="#" onClick={loginPlain}>Password</NavDropdown.Item>
                <NavDropdown.Item href="#" onClick={loginMfa}>TOTP [MFA]</NavDropdown.Item>
                <NavDropdown.Item href="#" onClick={loginHwk}>HW Key [HWK]</NavDropdown.Item>
            </NavDropdown>
            // <div className="dropdown">
            //     <button className="dropbtn">Login</button>
            //     <div className="dropdown-content">
            //     <a href="#" onClick={loginPlain}>Password</a>
            //     <a href="#" onClick={loginMfa}>TOTP [MFA]</a>
            //     <a href="#" onClick={loginHwk}>HW Key [HWK]</a>
            //     </div>
            //</div>
                // {/*
                //     The following link is used for the "Super Secret" page
                //     The scenario is when using three levels of Step-Up auth which are:
                //     - Password (pwd)
                //     - TOTP Google Authenticator (mfa)
                //     - Hardware FIDO2 key (hwk)
                //  */}

        );
    }
}


export default LoginControl;