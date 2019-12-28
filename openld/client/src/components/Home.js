import React, { Component, Fragment } from 'react';
import { Button, Container, Jumbotron } from 'reactstrap';
import authService from './api-authorization/AuthorizeService';

export class Home extends Component {
  static displayName = Home.name;

  constructor(props) {
    super(props);

    this.state = {
      actions: <div></div>
    }

    this.handleCreate = this.handleCreate.bind(this);
  }

  componentDidMount() {
    document.title = "OpenLD";
    this.renderActions();
  }

  render () {
    return (
      <Jumbotron>
        <Container>
          <h1 className="display-3">OpenLD</h1>
          <p>OpenLD is a free online tool for creating lighting designs collaboratively.</p>
        </Container>
        {this.state.actions}
      </Jumbotron>
    );
  }

  async handleCreate() {
    const response = await fetch("api/drawing/CreateDrawing", {
      method: "POST",
      headers: await authService.generateHeader()
    });

    const data = await response.json();

    if (data.success === true) {
      this.props.history.push("/drawing/" + data.data);
    }
  }

  async renderActions() {
    if (await authService.isAuthenticated()) {
    this.setState({actions: <Fragment><hr/><Button color="primary" size="lg" onClick={this.handleCreate}>Create Drawing</Button></Fragment>});
    }
  }
}
