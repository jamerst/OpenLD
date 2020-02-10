import React, { Component, Fragment } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Collapse,
  Col, Container, Row,
  Button, CustomInput, Form, FormGroup, Input, Label, Spinner } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import { FixtureResults } from './fixture-library/FixtureResults';
import { AddFixtureForm } from './fixture-library/AddFixtureForm';
import { SearchParams } from './fixture-library/SearchParams';

export class FixtureLibrary extends Component {
  static displayName = FixtureLibrary.name;

  constructor(props) {
    super(props);

    this.state = {
      results: [],
      types: [],
      defaultType: "",
      searchParams: new SearchParams(),
      advancedSearch: false,
      addButton: "",
      addModalOpen: false,
      loading: true,
      loadingTypes: true
    };
  }

  componentDidMount = () => {
    this.renderAddButton();
    document.title = "OpenLD Fixture Library";
    if (!this.props.embedded && this.props.match.params.term) {
      const params = this.props.location.search;
      this.setState({ searchParams: new SearchParams(params) });
      this.setState({ searchTerm: this.props.match.params.term });
      this.fetchFixtures(this.props.match.params.term);
    } else {
      this.fetchFixtures("");
    }
    this.fetchFixtureTypes();
  }

  static defaultProps = {
    embedded: false,
    xs: "12",
    md: "4",
    cardImgSize: "30rem",
    height: "100%",
    onCardClick: () => void(0)
  };

  render = () => {
    const toggle = () => this.setState({ advancedSearch: !this.state.advancedSearch });

    let contents = this.state.loading
      ? <div className="text-center"><Spinner style={{width: "5em", height: "5em"}}/></div>
      : <FixtureResults
        results={this.state.results}
        onCardClick={this.props.onCardClick}
        xs={this.props.xs}
        md={this.props.md}
        cardImgSize = {this.props.cardImgSize}
        height = {this.props.height}
      />;

    let types = this.renderTypes();

    let header = !this.props.embedded ?
      (
        <Container className="p-0">
        <Row className="justify-content-between align-items-center m-0">
          <Col className="p-0">
            <h1>Fixture Library</h1>
          </Col>

          <Col xs="12" md="auto" className="p-0 mb-1 mb-md-0">
            {this.state.addButton}
          </Col>
        </Row>
      </Container>
    )
    : null;

    return (
      <Fragment>
        {header}

        <Form onSubmit={this.handleSearchSubmit} onChange={this.handleSearchChange} >
          <FormGroup row>
            <Col>
              <Input type="text" value={this.state.searchParams.name} name="name" placeholder="Name" />
            </Col>

            <Col xs="auto" className="pl-0">
              <Button type="submit" color="primary" >Search</Button>
            </Col>
          </FormGroup>

          <Button color="link" size="sm" onClick={toggle}>More Options <FontAwesomeIcon icon="chevron-down" /></Button>

          <Collapse isOpen={this.state.advancedSearch}>
            <FormGroup row>
              <Col xs="12" md="6">
                <Input type="text" bsSize="sm" value={this.state.searchParams.manufacturer} name="manufacturer" placeholder="Manufacturer" />
              </Col>

              <Col xs="12" md="6" className="form-inline d-flex pl-md-0">
                <Label for="searchType" size="sm" className="mb-0 mr-2">Fixture Type</Label>
                <CustomInput type="select" name="type" id="searchType" bsSize="sm" value={this.state.searchParams.type} className="flex-grow-1">
                  <option value="">Any</option>
                  {types}
                </CustomInput>
              </Col>
            </FormGroup>
          </Collapse>
        </Form>

        {contents}

        <AddFixtureForm
          isOpen = {this.state.addModalOpen}
          types = {this.state.types}
          renderedTypes = {types}
          onSubmitSuccess = {() => {
            this.fetchFixtures();
            this.handleAddToggle();
          }}
          onCancel = {this.handleAddToggle}
        />
      </Fragment>
    );
  }

  handleSearchChange = (event) => {
    let params = this.state.searchParams;
    params[event.target.name] = event.target.value;
    this.setState({ searchParams: params });
  }

  handleSearchSubmit = (event) => {
    event.preventDefault();
    this.fetchFixtures(this.state.searchTerm);
    if (!this.props.embedded) {
      this.props.history.push(`/library/${this.state.searchParams.getQueryString()}`);
    }
  }

  handleAddToggle = () => {
    this.setState({ addModalOpen: !this.state.addModalOpen });
  }

  fetchFixtures = async () => {
    const response = await fetch('api/library/GetFixtures', {
      method: "POST",
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(this.state.searchParams)
    });

    const data = await response.json();
    this.setState({ results: data.data, loading: false });
  }

  renderTypes = () => {
    return (
      <Fragment>
        {this.state.types.map(type => {
          return <option key={type.id} value={type.id}>{type.name}</option>
        }
        )}
      </Fragment>
    );
  }

  fetchFixtureTypes = async () => {
    const response = await fetch('api/library/GetFixtureTypes', {
      method: "POST"
    });

    const data = await response.json();
    this.setState({ types: data.data, loadingTypes: false });
  }

  renderAddButton = async () => {
    if (await authService.isAuthenticated()) {
      this.setState({ addButton: <Button color="secondary" onClick={this.handleAddToggle}>Add Fixture</Button> });
    }
  }
}
