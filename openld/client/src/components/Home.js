import React, { Component, Fragment } from 'react';
import {
  Button, Container, Jumbotron
} from 'reactstrap';
import authService from './api-authorization/AuthorizeService';
import { CreateDrawingForm } from './drawing/CreateDrawingForm';

export class Home extends Component {
  static displayName = Home.name;

  constructor(props) {
    super(props);

    this.state = {
      actions: <div></div>,
      createModalOpen: false,
    }

    this.toggle = this.toggle.bind(this);
    this.redirect = this.redirect.bind(this);
    this.renderActions = this.renderActions.bind(this);
  }

  componentDidMount() {
    document.title = "OpenLD";
    this.renderActions();
  }

  render () {
    return (
      <Fragment>
        <Jumbotron>
          <Container>
            <h1 className="display-3">OpenLD</h1>
            <p>OpenLD is a free online tool for creating lighting designs collaboratively.</p>
          </Container>
          {this.state.actions}
        </Jumbotron>

        <CreateDrawingForm
          isOpen = {this.state.createModalOpen}
          onSubmitSuccess = {this.redirect}
          toggle = {this.toggle}
        />
      </Fragment>
    );
  }

  toggle() {
    this.setState({createModalOpen: !this.state.createModalOpen});
  }

  redirect(id) {
    this.props.history.push("/drawing/" + id);
  }

  async renderActions() {
    if (await authService.isAuthenticated()) {
      this.setState({actions: <Fragment><hr/><Button color="primary" size="lg" onClick={this.toggle}>Create Drawing</Button></Fragment>});
    }
  }
}
