
namespace openld.Utils {
    public class JsonResponse<T> {
        public bool success;
        public T data;
        public string msg;
        public int code;
    }
}