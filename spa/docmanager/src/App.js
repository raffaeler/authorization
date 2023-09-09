import logo from './logo.svg';
import './App.css';

import React, { useState } from 'react';
import { Routes, Route, Outlet, Link } from "react-router-dom";

import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';

import { LinkContainer } from "react-router-bootstrap";

// import { BrowserRouter as Router } from 'react-router-dom';
// import { Routes } from 'react-router-dom';

//import { useAuth } from "@axa-fr/react-oidc";
import { useOidc } from "@axa-fr/react-oidc";
import { useOidcIdToken, useOidcAccessToken } from '@axa-fr/react-oidc';

import ShowToken from './components/showToken';
import ShowJson from './components/showJson';
import LoginControl from './components/loginControl';
import DocCreate from './components/docCreate';
import DocList from './components/docList';
import DocDetails from './components/docDetails';

const acr_to_loa = Object.freeze({
  pwd: 1,
  mfa: 2,
  hwk: 3,
});

function App() {
  const { login, logout, renewTokens, isAuthenticated } = useOidc();
  const { idToken, idTokenPayload } = useOidcIdToken();
  const { accessToken, accessTokenPayload } = useOidcAccessToken();

  const [result, setResult] = useState("");
  const [isError, setIsError] = useState(true);

  const invokeAPI = async (resource, requested_loa, previousInvocationOk = true) => {
    try {
      console.log(`requesting ${resource} with loa:${requested_loa}`)
      const token = idToken;
      //console.log(token);
      if (!isAuthenticated) {
        setResult("User is not authenticated");
        setIsError(true);
        return;
      }

      let token_loa = acr_to_loa[accessTokenPayload.acr];
      if (token_loa < requested_loa) {
        setResult("User need higher privileges: " + Object.keys(acr_to_loa)[requested_loa - 1]);
        setIsError(true);
        return;
      }

      const response = await fetch("https://spa.iamraf.net:5001/api/documents/" + resource, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

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

        // //if(previousInvocationOk && response.status == 403) {
        // if(response.status == 403) {
        //   console.log("fetch access denied: logout+login in progress: " + loa);
        //   await logout();
        //   await login(null, {
        //     acr_values: loa
        //   });
        //   invokeAPI(resource, loa, false);
        //   return;
        // }

        setResult(message);
        setIsError(true);
        return;
      }

      setResult(await response.json());
      setIsError(false);
    }
    catch (e) {
      console.log(e);
      setResult(e.message);
      setIsError(true);
    }
  }

  const loggedOut = () => {
    setResult("");
    setIsError(true);
  }

  return (
    <Routes>
      <Route path='/' element={<Layout />}>
        <Route index element={<DocList />} />
        <Route path="debug" element={<ShowToken />} />
        <Route path="create" element={<DocCreate />} />
        <Route path="details/:id" element={<DocDetails />}/>
        <Route path="edit/:id" element={<DocDetails mode='edit' />}/>
      </Route>
    </Routes>
  );

  function Layout() {
    return (
      <>
      <Navbar bg="dark" data-bs-theme="dark">
        <Container>
          <Navbar.Brand>Documents Demo</Navbar.Brand>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="me-auto container-fluid">
              <LinkContainer to="/">
                <Nav.Link>Home</Nav.Link>
              </LinkContainer>
              <LinkContainer to="/debug">
                <Nav.Link>Debug</Nav.Link>
              </LinkContainer>
              <LinkContainer to="/create">
                <Nav.Link>Create</Nav.Link>
              </LinkContainer>
              <Nav.Item className="ms-auto">
                <LoginControl onLogout={loggedOut} />
              </Nav.Item>
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>

      <Outlet />
      </>
    );
  }

  function Layout2() {
    return (

      <div>
        {/* A "layout route" is a good place to put markup you want to
          share across all the pages on your site, like navigation. */}
        <nav>
          <ul className="nav-ul">
            <li className="nav-li">
              <Link to="/">Home</Link>
            </li>
            <li className="nav-li">
              <Link to="/debug">Debug</Link>
            </li>
            <li className="nav-li">
              <Link to="/create">Create</Link>
            </li>

            <li className="nav-li lir"  ><LoginControl onLogout={loggedOut} /></li>
          </ul>
        </nav>

        <hr />

        {/* An <Outlet> renders whatever child route is currently active,
          so you can think about this <Outlet> as a placeholder for
          the child routes we defined above. */}
        <Outlet />
      </div>
    )
  }

  function Home() {
    return (
      <div>
        <h2>Home</h2>
      </div>
    );
  }


}
export default App;
/*

    <div>
      <header className="header">
        <div className="one">
          <a href="#" className="apilink" onClick={() => invokeAPI("ValuesPlain", acr_to_loa.pwd)}>Plain API</a>
          <a href="#" className="apilink" onClick={() => invokeAPI("ValuesMfa", acr_to_loa.mfa)}>TOTP API</a>
          <a href="#" className="apilink" onClick={() => invokeAPI("ValuesHwk", acr_to_loa.hwk)}>Key API</a>
        </div>

      </header>


      <div className="apiResult">
        {isError ? result : (<ShowJson label="API result" data={result} />)}
      </div>







      {}
      </div>

*/
