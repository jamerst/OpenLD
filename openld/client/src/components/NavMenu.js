import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { LoginMenu } from './api-authorization/LoginMenu';
import './NavMenu.css';
import logo from '../res/logo.svg';

export class NavMenu extends Component {
  static displayName = NavMenu.name;

  constructor (props) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/"><img src={logo} /></NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow mr-auto h4">
                <NavItem>
                  <NavLink tag={Link} to="/library">Fixture Library</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} to="/drawing">Drawing</NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} to="/token">Token</NavLink>
                </NavItem>
              </ul>
              <ul className="navbar-nav h5">
                <LoginMenu>
                </LoginMenu>
              </ul>
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}
