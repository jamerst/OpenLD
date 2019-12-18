import React, { Component } from 'react';
import { Container } from 'reactstrap';
import authService from './api-authorization/AuthorizeService';

export class Token extends Component {
    constructor(props) {
        super(props);
        this.state = {
            token: ""
        };
    }

    componentDidMount() {
        this.getToken();
    }

    render() {
        return (
            <Container>Access token: <code>{this.state.token}</code></Container>
        );
    }

    async getToken() {
        if (await authService.isAuthenticated()) {
            this.setState({token: await authService.getAccessToken()});
        } else {
            this.setState({token: "not logged in"})
        }
    }
}