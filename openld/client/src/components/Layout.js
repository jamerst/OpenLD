import React, { Component, Fragment } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

export class PageLayout extends Component {
  static displayName = PageLayout.name;

  render () {
    return (
      <Fragment>
        <NavMenu />
        <Container>
          {this.props.children}
        </Container>
      </Fragment>
    );
  }
}

export class DrawingLayout extends Component {
  static displayName = DrawingLayout.name;

  render = () => {
    return (
      <Container fluid className="p-0 h-100">
        {this.props.children}
      </Container>
    );
  }
}
