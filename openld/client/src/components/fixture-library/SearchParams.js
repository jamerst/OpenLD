export class SearchParams {
    name = "";
    manufacturer = "";
    type = "";

    constructor(searchParams) {
        const params = new URLSearchParams(searchParams);

        this.name = params.get("name") !== null ? params.get("name") : "";
        this.manufacturer = params.get("manufacturer") !== null ? params.get("manufacturer") : "";
        this.type = params.get("type") !== null ? params.get("type") : "";
    }

    setName(name) {
        this.name = name;
    }

    setManf(manf) {
        this.manufacturer = manf;
    }

    setType(type) {
        this.type = type;
    }

    getQueryString() {
        return "?" + Object.keys(this).map(key => {
            return encodeURIComponent(key) + "=" + encodeURIComponent(this[key])
        }).join("&");
    }
}