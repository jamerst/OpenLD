import React, { Component } from 'react';
import { Container } from 'reactstrap';

export class Home extends Component {
  static displayName = Home.name;

  componentDidMount() {
    document.title = "OpenLD";
  }

  render () {
    return (
      <Container>
        <div className="jumbotron">
          <div className="container">
            <h1 className="display-3">OpenLD</h1>
            <p>OpenLD is a free online tool for creating lighting designs collaboratively.</p>
          </div>
        </div>
      </Container>
    );
  }
}
