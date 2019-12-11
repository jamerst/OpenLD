import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Button, Collapse } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import { FixtureResults } from './fixture-library/FixtureResults';
import { SearchParams } from './fixture-library/SearchParams';

export class FixtureLibrary extends Component {
  static displayName = FixtureLibrary.name;

  constructor(props) {
    super(props);
    this.state = {
      results: [],
      types: [],
      searchParams: new SearchParams(),
      advancedSearch: false,
      loading: true,
      loadingTypes: true
    };

    this.handleName = this.handleName.bind(this);
    this.handleManf = this.handleManf.bind(this);
    this.handleType = this.handleType.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.fetchFixtures = this.fetchFixtures.bind(this);
  }

  componentDidMount() {
    const params = this.props.location.search;
    console.log("PARAMS:" + params);
    this.setState({ searchParams: new SearchParams(params) });
    document.title = "OpenLD Fixture Library";
    if (this.props.match.params.term) {
      this.setState({ searchTerm: this.props.match.params.term });
      this.fetchFixtures(this.props.match.params.term);
    } else {
      this.fetchFixtures("");
    }
    this.fetchFixtureTypes();
  }

  render() {
    const toggle = () => this.setState({ advancedSearch: !this.state.advancedSearch });

    let contents = this.state.loading
      ? <p className="text-center"><em>Loading results..</em></p>
      : FixtureResults.renderResults(this.state.results);

    let types = this.renderTypes();

    return (
      <div>
        <div className="container">
          <div className="row justify-content-between align-items-center">
            <div className="col">
              <h1>Fixture Library</h1>
            </div>

            <div className="col-auto">
              <button className="btn btn-secondary">Add Fixture</button>
            </div>
          </div>
        </div>

        <form onSubmit={this.handleSubmit}>
          <div className="form-row">
            <div className="col-11">
              <input type="text" className="form-control" value={this.state.searchParams.name} onChange={this.handleName} placeholder="Name" />
            </div>

            <div className="col">
              <button type="submit" className="btn btn-primary">Search</button>
            </div>
          </div>

          <Button color="link" size="sm" onClick={toggle}>More Options <FontAwesomeIcon icon="chevron-down" /></Button>

          <Collapse isOpen={this.state.advancedSearch}>
            <div className="form-row">
              <div className="col">
                <input type="text" className="form-control form-control-sm" value={this.state.searchParams.manufacturer} onChange={this.handleManf} placeholder="Manufacturer" />
              </div>

              <div className="col form-group">
                <label htmlFor="searchType" className="col-auto col-form-label form-control-sm">Fixture Type</label>
                <select name="type" id="searchType" value={this.state.searchParams.type} onChange={this.handleType} className="custom-select col-9 custom-select-sm">
                  {types}
                </select>
              </div>
            </div>
          </Collapse>
        </form>

        {contents}
      </div>
    );
  }

  handleName(event) {
    let params = this.state.searchParams;
    params.setName(event.target.value);
    this.setState({ searchParams: params });
  }

  handleManf(event) {
    let params = this.state.searchParams;
    params.setManf(event.target.value);
    this.setState({ searchParams: params });
  }

  handleType(event) {
    let params = this.state.searchParams;
    params.setType(event.target.value);
    this.setState({ searchParams: params });
  }

  handleSubmit(event) {
    event.preventDefault();
    this.fetchFixtures(this.state.searchTerm);
    this.props.history.push("/library" + this.state.searchParams.getQueryString());
  }

  async fetchFixtures() {
    const response = await fetch('api/library/GetFixtures', {
      method: "POST",
      headers: { ...authService.generateAuthHeader(), ...{ 'Content-Type': 'application/json' } },
      body: JSON.stringify(this.state.searchParams)
    });

    const data = await response.json();
    this.setState({ results: data.data, loading: false });
  }

  renderTypes() {
    return (
      <React.Fragment>
        <option value="">Any</option>
        {this.state.types.map(type =>
          <option value={type.id}>{type.name}</option>
        )}
      </React.Fragment>
    );
  }

  async fetchFixtureTypes() {
    const response = await fetch('api/library/GetFixtureTypes', {
      method: "POST",
      headers: authService.generateAuthHeader()
    });

    const data = await response.json();
    this.setState({ types: data.data, loadingTypes: false });
  }
}
